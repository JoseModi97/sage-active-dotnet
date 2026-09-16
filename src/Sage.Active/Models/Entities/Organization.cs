using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.Entities
{
    public class Organization
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("socialName")]
        public string? SocialName { get; set; }

        [JsonPropertyName("legislationCode")]
        public string? LegislationCode { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("onboardingCompleted")]
        public bool OnboardingCompleted { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTimeOffset? CreationDate { get; set; }
    }

    public class UserProfile
    {
        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("authenticationEmail")]
        public string? AuthenticationEmail { get; set; }

        [JsonPropertyName("applicationLanguageCode")]
        public string? ApplicationLanguageCode { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("authenticationEmail")]
        public string? AuthenticationEmail { get; set; }

        [JsonPropertyName("applicationLanguageCode")]
        public string? ApplicationLanguageCode { get; set; }

        [JsonPropertyName("auth0UserId")]
        public string? Auth0UserId { get; set; }
    }

    public class UserAccessPolicyCheckResult
    {
        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("isAllowed")]
        public bool IsAllowed { get; set; }
    }

    public class Country
    {
        [JsonPropertyName("isoCodeAlpha2")]
        public string IsoCodeAlpha2 { get; set; } = string.Empty;

        [JsonPropertyName("isoCodeAlpha3")]
        public string? IsoCodeAlpha3 { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class Currency
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }

    public class ZipCode
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }
    }
}
