using System;

namespace Sage.Active.Models
{
    /// <summary>
    /// Configuration options for connecting to the Sage Active Public API.
    /// </summary>
    public class SageActiveConfig
    {
        /// <summary>
        /// The target region (FR, ES, DE, PT). Defaults to FR.
        /// </summary>
        public SageRegion Region { get; set; } = SageRegion.FR;

        /// <summary>
        /// The target environment (Sandbox or Production). Defaults to Production.
        /// </summary>
        public SageEnvironment Environment { get; set; } = SageEnvironment.Production;

        /// <summary>
        /// Base API Gateway URL. Auto-computed from Region if left blank.
        /// </summary>
        public string? BaseAddress { get; set; }

        /// <summary>
        /// OAuth 2.0 Authorization Server URL. Defaults to https://sbcauth.sage.fr/connect/authorize.
        /// </summary>
        public string? AuthUrl { get; set; }

        /// <summary>
        /// OAuth 2.0 Access Token Server URL. Defaults to https://sbcauth.sage.fr/connect/token.
        /// </summary>
        public string? AccessTokenUrl { get; set; }

        /// <summary>
        /// Registered Application Client ID obtained from the Sage Developer Portal.
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// Registered Application Client Secret.
        /// </summary>
        public string? ClientSecret { get; set; }

        /// <summary>
        /// API Gateway Subscription Key passed as the 'x-api-key' header.
        /// </summary>
        public string? SubscriptionKey { get; set; }

        /// <summary>
        /// Active Organization Identifier passed as the 'X-OrganizationId' header.
        /// </summary>
        public string? OrganizationId { get; set; }

        /// <summary>
        /// Optional static Bearer Access Token if acquired externally.
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Optional Refresh Token for automated token renewal.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// OAuth Scopes requested during authorization.
        /// </summary>
        public string Scopes { get; set; } = "openid offline_access profile email SageActive";

        /// <summary>
        /// OAuth Redirect / Callback URL.
        /// </summary>
        public string? CallbackUrl { get; set; }

        /// <summary>
        /// HTTP Request Timeout. Defaults to 60 seconds.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Maximum retry attempts when rate limited (HTTP 429). Defaults to 3.
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Resolves the effective Base Address.
        /// </summary>
        public string GetEffectiveBaseAddress()
        {
            if (!string.IsNullOrWhiteSpace(BaseAddress))
            {
                var addr = BaseAddress!.TrimEnd('/');
                return addr.EndsWith("/graphql", StringComparison.OrdinalIgnoreCase) 
                    ? addr.Substring(0, addr.Length - 8) 
                    : addr;
            }
            return RegionalEndpoints.GetBaseAddress(Region, Environment);
        }

        /// <summary>
        /// Resolves the effective Token URL.
        /// </summary>
        public string GetEffectiveTokenUrl()
        {
            return !string.IsNullOrWhiteSpace(AccessTokenUrl)
                ? AccessTokenUrl!
                : RegionalEndpoints.GetTokenUrl(Region);
        }

        /// <summary>
        /// Resolves the effective Auth URL.
        /// </summary>
        public string GetEffectiveAuthUrl()
        {
            return !string.IsNullOrWhiteSpace(AuthUrl)
                ? AuthUrl!
                : RegionalEndpoints.GetAuthUrl(Region);
        }

        /// <summary>
        /// Validates that minimum required credentials exist.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(SubscriptionKey) && string.IsNullOrWhiteSpace(AccessToken))
            {
                throw new InvalidOperationException("Either 'SubscriptionKey' (for x-api-key) or a valid 'AccessToken' must be provided in SageActiveConfig.");
            }
        }
    }
}
