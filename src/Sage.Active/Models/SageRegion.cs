using System;

namespace Sage.Active.Models
{
    /// <summary>
    /// Supported geographic regions / legislations in Sage Active.
    /// </summary>
    public enum SageRegion
    {
        /// <summary>France (FR)</summary>
        FR,

        /// <summary>Spain (ES)</summary>
        ES,

        /// <summary>Germany (DE)</summary>
        DE,

        /// <summary>Portugal (PT)</summary>
        PT
    }

    /// <summary>
    /// Environment targets: Sandbox (Developer Testing) or Production (Live Operations).
    /// </summary>
    public enum SageEnvironment
    {
        /// <summary>Production live operations.</summary>
        Production,

        /// <summary>Developer sandbox environment.</summary>
        Sandbox
    }

    /// <summary>
    /// Regional endpoint address resolver for Sage Active APIs and SBC Auth.
    /// </summary>
    public static class RegionalEndpoints
    {
        public const string DefaultAuthUrl = "https://sbcauth.sage.fr/connect/authorize";
        public const string DefaultTokenUrl = "https://sbcauth.sage.fr/connect/token";

        public static string GetBaseAddress(SageRegion region, SageEnvironment environment = SageEnvironment.Production)
        {
            // Regional base API gateways
            switch (region)
            {
                case SageRegion.FR:
                    return "https://api.fr.active.sage.com";
                case SageRegion.ES:
                case SageRegion.PT:
                    return "https://api.es.active.sage.com";
                case SageRegion.DE:
                    return "https://api.de.active.sage.com";
                default:
                    throw new ArgumentOutOfRangeException(nameof(region), $"Unsupported Sage region: {region}");
            }
        }

        public static string GetAuthUrl(SageRegion region)
        {
            return DefaultAuthUrl;
        }

        public static string GetTokenUrl(SageRegion region)
        {
            return DefaultTokenUrl;
        }
    }
}
