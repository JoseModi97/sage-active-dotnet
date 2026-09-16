using System;
using System.IO;
using System.Text.Json;
using Sage.Active.Models;

namespace Sage.Active.Cli
{
    public static class CliConfigLoader
    {
        private const string ConfigFileName = "sageactive.config.json";

        public static SageActiveConfig LoadConfig()
        {
            var config = new SageActiveConfig();

            // 1. Try local config file
            if (File.Exists(ConfigFileName))
            {
                try
                {
                    var json = File.ReadAllText(ConfigFileName);
                    var parsed = JsonSerializer.Deserialize<SageActiveConfig>(json);
                    if (parsed != null) return parsed;
                }
                catch { }
            }

            // 2. Try appsettings.json
            if (File.Exists("appsettings.json"))
            {
                try
                {
                    using var doc = JsonDocument.Parse(File.ReadAllText("appsettings.json"));
                    if (doc.RootElement.TryGetProperty("SageActive", out var section))
                    {
                        if (section.TryGetProperty("SubscriptionKey", out var key)) config.SubscriptionKey = key.GetString();
                        if (section.TryGetProperty("OrganizationId", out var org)) config.OrganizationId = org.GetString();
                        if (section.TryGetProperty("ClientId", out var cid)) config.ClientId = cid.GetString();
                        if (section.TryGetProperty("ClientSecret", out var sec)) config.ClientSecret = sec.GetString();
                        if (section.TryGetProperty("AccessToken", out var tok)) config.AccessToken = tok.GetString();
                        if (section.TryGetProperty("Region", out var reg) && Enum.TryParse<SageRegion>(reg.GetString(), true, out var r)) config.Region = r;
                        if (section.TryGetProperty("Environment", out var env) && Enum.TryParse<SageEnvironment>(env.GetString(), true, out var e)) config.Environment = e;
                        return config;
                    }
                }
                catch { }
            }

            // 3. Fallback to Environment Variables
            var envKey = Environment.GetEnvironmentVariable("SAGE_ACTIVE_KEY") ?? Environment.GetEnvironmentVariable("SAGE_SUBSCRIPTION_KEY");
            if (!string.IsNullOrWhiteSpace(envKey)) config.SubscriptionKey = envKey;

            var envOrg = Environment.GetEnvironmentVariable("SAGE_ACTIVE_ORG_ID");
            if (!string.IsNullOrWhiteSpace(envOrg)) config.OrganizationId = envOrg;

            var envToken = Environment.GetEnvironmentVariable("SAGE_ACTIVE_TOKEN");
            if (!string.IsNullOrWhiteSpace(envToken)) config.AccessToken = envToken;

            return config;
        }

        public static void SaveConfig(SageActiveConfig config)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(ConfigFileName, json);
        }
    }
}
