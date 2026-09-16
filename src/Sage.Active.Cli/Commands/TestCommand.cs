using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage.Active.Cli.Commands
{
    public static class TestCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            var config = CliConfigLoader.LoadConfig();

            Prompter.Info($"Testing Sage Active connection for Region: {config.Region} ({config.Environment})...");
            Prompter.Info($"Base Address: {config.GetEffectiveBaseAddress()}");

            try
            {
                var client = new SageActiveClient(config);

                // 1. Check User Profile
                Prompter.Info("Verifying User Profile...");
                var profile = await client.Users.GetUserProfileAsync();
                Prompter.Success($"Connected as: {profile.FullName ?? profile.FirstName ?? "Sage User"} ({profile.AuthenticationEmail})");

                // 2. Check Organizations
                Prompter.Info("Verifying Organization Access...");
                var orgs = await client.Organizations.GetOrganizationsAsync(5);
                Prompter.Success($"Active Organization ID: {config.OrganizationId ?? (orgs.TotalCount > 0 ? "First available" : "None")}");

                // 3. Check User Access Policy
                Prompter.Info("Checking User Access Policies...");
                var testActions = new[]
                {
                    "organizations",
                    "accountingAccounts",
                    "accountingEntries",
                    "salesInvoices",
                    "purchaseInvoices",
                    "customers"
                };

                var policies = await client.Users.CheckAccessPolicyAsync(testActions);
                var rows = new List<List<string>>();
                foreach (var p in policies)
                {
                    rows.Add(new List<string> { p.Action, p.IsAllowed ? "ALLOWED" : "DENIED" });
                }

                TableFormatter.PrintTable(new[] { "Action", "Permission" }, rows);
                Prompter.Success("All diagnostic checks completed successfully! Your environment is ready.");
            }
            catch (Exception ex)
            {
                Prompter.Error($"Diagnostic test failed: {ex.Message}");
            }
        }
    }
}
