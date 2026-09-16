using System;
using System.Text.Json.Serialization;

namespace Sage.Active.Models
{
    /// <summary>
    /// Represents an OAuth 2.0 Token response from Sage Business Cloud Auth.
    /// </summary>
    public class SageToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        /// <summary>
        /// Local timestamp when this token was acquired.
        /// </summary>
        public DateTimeOffset AcquiredAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// True if the access token is expired or within 60 seconds of expiring.
        /// </summary>
        public bool IsExpired => DateTimeOffset.UtcNow >= AcquiredAt.AddSeconds(Math.Max(0, ExpiresIn - 60));
    }
}
