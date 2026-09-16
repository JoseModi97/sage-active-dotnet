using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Sage.Active.Models;

namespace Sage.Active.Cli.Commands
{
    public static class InitCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=================================================");
            Console.WriteLine("       Sage Active CLI Setup & Scaffolder        ");
            Console.WriteLine("=================================================");
            Console.ResetColor();
            Console.WriteLine("This wizard will configure your Sage Active API connection.\n");

            var config = CliConfigLoader.LoadConfig();

            // 1. Region
            var regionStr = Prompter.Prompt("Select Sage Region (FR, ES, DE, PT)", config.Region.ToString()).ToUpper();
            if (Enum.TryParse<SageRegion>(regionStr, true, out var reg))
            {
                config.Region = reg;
            }

            // 2. Environment
            var envStr = Prompter.Prompt("Select Environment (Sandbox, Production)", config.Environment.ToString());
            if (Enum.TryParse<SageEnvironment>(envStr, true, out var env))
            {
                config.Environment = env;
            }

            // 3. Credentials
            config.SubscriptionKey = Prompter.Prompt("Enter API Subscription Key (x-api-key)", config.SubscriptionKey ?? "");
            config.ClientId = Prompter.Prompt("Enter Client ID (from Developer Portal)", config.ClientId ?? "");
            config.ClientSecret = Prompter.Prompt("Enter Client Secret", config.ClientSecret ?? "");
            config.AccessToken = Prompter.Prompt("Enter Bearer Access Token (or press enter if using SBC Auth)", config.AccessToken ?? "");

            // 4. Test connection & query organizations
            Prompter.Info("Connecting to Sage Active to fetch your organizations...");
            try
            {
                var client = new SageActiveClient(config);
                var orgs = await client.Organizations.GetOrganizationsAsync(10);

                if (orgs.TotalCount > 0)
                {
                    Prompter.Success($"Found {orgs.TotalCount} organization(s):");
                    var index = 1;
                    foreach (var org in orgs.Nodes)
                    {
                        Console.WriteLine($"  [{index++}] {org.SocialName} ({org.LegislationCode}) - ID: {org.Id}");
                    }

                    var pick = Prompter.Prompt("Select default organization number", "1");
                    if (int.TryParse(pick, out var p) && p >= 1 && p <= orgs.TotalCount)
                    {
                        var i = 1;
                        foreach (var org in orgs.Nodes)
                        {
                            if (i++ == p)
                            {
                                config.OrganizationId = org.Id;
                                Prompter.Success($"Selected: {org.SocialName} ({org.Id})");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Prompter.Warning("No organizations were found for this account. You can enter an Organization ID manually.");
                    config.OrganizationId = Prompter.Prompt("Organization ID (UUID)", config.OrganizationId ?? "");
                }
            }
            catch (Exception ex)
            {
                Prompter.Warning($"Unable to automatically query organizations: {ex.Message}");
                config.OrganizationId = Prompter.Prompt("Enter Organization ID (UUID) manually", config.OrganizationId ?? "");
            }

            // Save configuration
            CliConfigLoader.SaveConfig(config);
            Prompter.Success("Saved configuration to sageactive.config.json!");

            // Also write or update appsettings.json if present
            if (File.Exists("appsettings.json") || Prompter.Prompt("Write to appsettings.json? (y/n)", "y").ToLower() == "y")
            {
                UpdateAppSettings(config);
                Prompter.Success("Updated appsettings.json with SageActive configuration!");
            }

            Console.WriteLine();
            Prompter.Info("Run 'sage-active test' to verify your connection and permissions!");
        }

        private static void UpdateAppSettings(SageActiveConfig config)
        {
            var appSettings = new
            {
                SageActive = new
                {
                    Region = config.Region.ToString(),
                    Environment = config.Environment.ToString(),
                    SubscriptionKey = config.SubscriptionKey,
                    OrganizationId = config.OrganizationId,
                    ClientId = config.ClientId,
                    ClientSecret = config.ClientSecret,
                    AccessToken = config.AccessToken
                }
            };

            var json = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("appsettings.json", json);
        }
    }
}
