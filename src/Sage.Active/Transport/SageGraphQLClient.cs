using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Auth;
using Sage.Active.Exceptions;
using Sage.Active.Models;
using Sage.Active.Models.GraphQL;

namespace Sage.Active.Transport
{
    /// <summary>
    /// Core transport engine executing GraphQL queries and mutations against the Sage Active Public API.
    /// </summary>
    public class SageGraphQLClient
    {
        private readonly HttpClient _httpClient;
        private readonly SageActiveConfig _config;
        private readonly SageAuthClient _authClient;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public SageGraphQLClient(SageActiveConfig config, HttpClient? httpClient = null, SageAuthClient? authClient = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.Timeout = _config.Timeout;
            _authClient = authClient ?? new SageAuthClient(_config, _httpClient);
        }

        public SageActiveConfig Config => _config;
        public SageAuthClient Auth => _authClient;

        /// <summary>
        /// Executes a GraphQL query or mutation and returns the deserialized data envelope.
        /// </summary>
        public async Task<TData> SendQueryAsync<TData>(string query, object? variables = null, string? operationName = null, CancellationToken cancellationToken = default)
        {
            var request = new GraphQLRequest(query, variables, operationName);
            var response = await SendRequestAsync<TData>(request, cancellationToken).ConfigureAwait(false);

            if (response.HasErrors)
            {
                var firstError = response.Errors![0];
                throw new SageApiException($"Sage Active GraphQL Error: {firstError.Message}", errors: response.Errors);
            }

            if (response.Data == null)
            {
                throw new SageApiException("Sage Active returned empty data without errors.");
            }

            return response.Data;
        }

        /// <summary>
        /// Sends a raw GraphQLRequest payload with automatic retry on HTTP 429.
        /// </summary>
        public async Task<GraphQLResponse<TData>> SendRequestAsync<TData>(GraphQLRequest graphQLRequest, CancellationToken cancellationToken = default)
        {
            var endpointUrl = $"{_config.GetEffectiveBaseAddress()}/graphql";
            var jsonPayload = JsonSerializer.Serialize(graphQLRequest, JsonOptions);

            var attempts = 0;
            var maxAttempts = Math.Max(1, _config.MaxRetryAttempts);

            while (true)
            {
                attempts++;
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl)
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                };

                await ApplyHeadersAsync(requestMessage, cancellationToken).ConfigureAwait(false);

                HttpResponseMessage? responseMessage = null;
                try
                {
                    responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);

                    if (responseMessage.StatusCode == (HttpStatusCode)429)
                    {
                        if (attempts >= maxAttempts)
                        {
                            throw new SageRateLimitException("Sage Active rate limit exceeded (3000 requests/minute). Maximum retry attempts reached.");
                        }

                        var delay = GetRetryDelay(responseMessage, attempts);
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    var responseBody = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        throw new SageApiException(
                            $"Sage Active API call failed with status {(int)responseMessage.StatusCode} ({responseMessage.StatusCode}): {responseBody}",
                            (int)responseMessage.StatusCode);
                    }

                    var graphQLResponse = JsonSerializer.Deserialize<GraphQLResponse<TData>>(responseBody, JsonOptions);
                    return graphQLResponse ?? throw new SageApiException("Failed to deserialize GraphQL response.");
                }
                finally
                {
                    responseMessage?.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a GraphQL multipart file upload adhering to the GraphQL Multipart Request Specification.
        /// </summary>
        public async Task<TData> SendMultipartAsync<TData>(string query, object variables, Stream fileStream, string fileName, string contentType = "application/octet-stream", CancellationToken cancellationToken = default)
        {
            var endpointUrl = $"{_config.GetEffectiveBaseAddress()}/graphql";

            using var form = new MultipartFormDataContent();

            // 1. operations part
            var operations = JsonSerializer.Serialize(new
            {
                query = query,
                variables = variables
            }, JsonOptions);
            form.Add(new StringContent(operations, Encoding.UTF8, "application/json"), "operations");

            // 2. map part
            var map = "{\"0\": [\"variables.input.file\"]}";
            form.Add(new StringContent(map, Encoding.UTF8, "application/json"), "map");

            // 3. file part
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(fileContent, "0", fileName);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl)
            {
                Content = form
            };

            await ApplyHeadersAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            using var responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
            var responseBody = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new SageApiException($"Multipart upload failed with status {(int)responseMessage.StatusCode}: {responseBody}", (int)responseMessage.StatusCode);
            }

            var graphQLResponse = JsonSerializer.Deserialize<GraphQLResponse<TData>>(responseBody, JsonOptions);
            if (graphQLResponse == null || graphQLResponse.HasErrors)
            {
                var error = graphQLResponse?.Errors?[0].Message ?? "Unknown multipart error";
                throw new SageApiException($"Sage Active file upload failed: {error}", errors: graphQLResponse?.Errors);
            }

            return graphQLResponse.Data!;
        }

        private async Task ApplyHeadersAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            // API Gateway key
            if (!string.IsNullOrWhiteSpace(_config.SubscriptionKey))
            {
                requestMessage.Headers.TryAddWithoutValidation("x-api-key", _config.SubscriptionKey);
            }

            // Organization ID (tenant context)
            if (!string.IsNullOrWhiteSpace(_config.OrganizationId))
            {
                requestMessage.Headers.TryAddWithoutValidation("X-OrganizationId", _config.OrganizationId);
            }

            // OAuth Bearer Token
            var token = await _authClient.GetValidAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private static TimeSpan GetRetryDelay(HttpResponseMessage response, int attempt)
        {
            if (response.Headers.RetryAfter != null)
            {
                if (response.Headers.RetryAfter.Delta.HasValue)
                {
                    return response.Headers.RetryAfter.Delta.Value;
                }
            }
            // Exponential backoff fallback: 1s, 2s, 4s...
            return TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
        }
    }
}
