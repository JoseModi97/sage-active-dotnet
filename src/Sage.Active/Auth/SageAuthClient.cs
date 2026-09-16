using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Exceptions;
using Sage.Active.Models;

namespace Sage.Active.Auth
{
    /// <summary>
    /// Handles OAuth 2.0 token acquisition and refresh via Sage Business Cloud Auth.
    /// </summary>
    public class SageAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly SageActiveConfig _config;
        private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);
        private SageToken? _cachedToken;

        public SageAuthClient(SageActiveConfig config, HttpClient? httpClient = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpClient = httpClient ?? new HttpClient();

            if (!string.IsNullOrWhiteSpace(_config.AccessToken))
            {
                _cachedToken = new SageToken
                {
                    AccessToken = _config.AccessToken!,
                    RefreshToken = _config.RefreshToken,
                    ExpiresIn = 3600,
                    AcquiredAt = DateTimeOffset.UtcNow
                };
            }
        }

        /// <summary>
        /// Builds the browser authorization URL for SBC Auth login.
        /// </summary>
        public string BuildAuthorizationUrl(string state, string? redirectUri = null)
        {
            var authUrl = _config.GetEffectiveAuthUrl();
            var callback = redirectUri ?? _config.CallbackUrl ?? "http://localhost:5000/callback";

            return $"{authUrl}?client_id={Uri.EscapeDataString(_config.ClientId ?? "")}" +
                   $"&response_type=code" +
                   $"&scope={Uri.EscapeDataString(_config.Scopes)}" +
                   $"&redirect_uri={Uri.EscapeDataString(callback)}" +
                   $"&state={Uri.EscapeDataString(state)}";
        }

        /// <summary>
        /// Exchanges an authorization code for an OAuth 2.0 token.
        /// </summary>
        public async Task<SageToken> ExchangeCodeAsync(string code, string? redirectUri = null, CancellationToken cancellationToken = default)
        {
            var tokenUrl = _config.GetEffectiveTokenUrl();
            var callback = redirectUri ?? _config.CallbackUrl ?? "http://localhost:5000/callback";

            var pairs = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = callback,
                ["client_id"] = _config.ClientId ?? "",
                ["client_secret"] = _config.ClientSecret ?? ""
            };

            return await RequestTokenAsync(tokenUrl, pairs, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Refreshes an expired access token using a refresh token.
        /// </summary>
        public async Task<SageToken> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var tokenUrl = _config.GetEffectiveTokenUrl();

            var pairs = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken,
                ["client_id"] = _config.ClientId ?? "",
                ["client_secret"] = _config.ClientSecret ?? ""
            };

            return await RequestTokenAsync(tokenUrl, pairs, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a valid access token, refreshing automatically if necessary.
        /// </summary>
        public async Task<string> GetValidAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            if (_cachedToken != null && !_cachedToken.IsExpired)
            {
                return _cachedToken.AccessToken;
            }

            await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_cachedToken != null && !_cachedToken.IsExpired)
                {
                    return _cachedToken.AccessToken;
                }

                if (!string.IsNullOrWhiteSpace(_cachedToken?.RefreshToken))
                {
                    var refreshed = await RefreshTokenAsync(_cachedToken!.RefreshToken!, cancellationToken).ConfigureAwait(false);
                    _cachedToken = refreshed;
                    _config.AccessToken = refreshed.AccessToken;
                    _config.RefreshToken = refreshed.RefreshToken;
                    return refreshed.AccessToken;
                }

                if (!string.IsNullOrWhiteSpace(_config.RefreshToken))
                {
                    var refreshed = await RefreshTokenAsync(_config.RefreshToken!, cancellationToken).ConfigureAwait(false);
                    _cachedToken = refreshed;
                    _config.AccessToken = refreshed.AccessToken;
                    _config.RefreshToken = refreshed.RefreshToken;
                    return refreshed.AccessToken;
                }

                if (!string.IsNullOrWhiteSpace(_config.AccessToken))
                {
                    return _config.AccessToken!;
                }

                throw new SageAuthException("No active access token or refresh token available. Run authentication or provide AccessToken/RefreshToken in SageActiveConfig.");
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        private async Task<SageToken> RequestTokenAsync(string tokenUrl, Dictionary<string, string> formValues, CancellationToken cancellationToken)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
                {
                    Content = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string?, string?>>)formValues)
                };

                using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new SageAuthException($"SBC Auth failed with status {(int)response.StatusCode}: {content}", (int)response.StatusCode);
                }

                var token = JsonSerializer.Deserialize<SageToken>(content)
                    ?? throw new SageAuthException("Failed to deserialize SBC Auth response.");

                token.AcquiredAt = DateTimeOffset.UtcNow;
                _cachedToken = token;
                _config.AccessToken = token.AccessToken;
                if (!string.IsNullOrWhiteSpace(token.RefreshToken))
                {
                    _config.RefreshToken = token.RefreshToken;
                }

                return token;
            }
            catch (Exception ex) when (!(ex is SageApiException))
            {
                throw new SageAuthException($"Error communicating with Sage Auth endpoint: {ex.Message}", innerException: ex);
            }
        }
    }
}
