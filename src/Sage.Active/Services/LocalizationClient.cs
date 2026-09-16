using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class LocalizationClient
    {
        private readonly SageGraphQLClient _transport;

        public LocalizationClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves localized error message by error code and target language (e.g. "FR", "ES", "DE", "EN").
        /// </summary>
        public async Task<string> GetLocalizedErrorMessageAsync(string errorCode, string language = "EN", CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($errorCode: String!, $language: String!) {
                localizedErrorMessage(errorCode: $errorCode, language: $language) {
                    message
                }
            }";

            var variables = new { errorCode, language };
            var result = await _transport.SendQueryAsync<ErrorMessageResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.LocalizedErrorMessage?.Message ?? errorCode;
        }

        private class ErrorMessageResponse
        {
            [JsonPropertyName("localizedErrorMessage")]
            public LocalizedMessage? LocalizedErrorMessage { get; set; }
        }

        private class LocalizedMessage
        {
            [JsonPropertyName("message")]
            public string Message { get; set; } = string.Empty;
        }
    }
}
