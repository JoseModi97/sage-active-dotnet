using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage.Active.Cli.Commands
{
    public static class OrgCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            var config = CliConfigLoader.LoadConfig();
            var client = new SageActiveClient(config);

            Prompter.Info("Retrieving organizations...");
            try
            {
                var orgs = await client.Organizations.GetOrganizationsAsync(20);

                var rows = new List<List<string>>();
                foreach (var o in orgs.Nodes)
                {
                    var isCurrent = string.Equals(o.Id, config.OrganizationId, StringComparison.OrdinalIgnoreCase);
                    rows.Add(new List<string>
                    {
                        o.Id,
                        o.SocialName ?? "",
                        o.LegislationCode ?? "",
                        o.Status ?? "",
                        isCurrent ? "YES (*)" : "NO"
                    });
                }

                TableFormatter.PrintTable(new[] { "ID", "Name", "Legislation", "Status", "Active" }, rows);

                if (args.Length > 1 && args[0].ToLower() == "set")
                {
                    var newOrgId = args[1];
                    config.OrganizationId = newOrgId;
                    CliConfigLoader.SaveConfig(config);
                    Prompter.Success($"Active organization switched to: {newOrgId}");
                }
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch organizations: {ex.Message}");
            }
        }
    }
}
