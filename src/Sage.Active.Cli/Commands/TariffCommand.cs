using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage.Active.Cli.Commands
{
    public static class TariffCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            var config = CliConfigLoader.LoadConfig();
            var client = new SageActiveClient(config);

            Prompter.Info("Retrieving sales tariffs & pricing...");
            try
            {
                var tariffs = await client.Products.GetTariffsAsync(15);
                var rows = new List<List<string>>();

                foreach (var t in tariffs.Nodes)
                {
                    rows.Add(new List<string>
                    {
                        t.Code,
                        t.Name ?? "",
                        t.Type ?? "",
                        t.Enabled.HasValue ? (t.Enabled.Value ? "Active" : "Disabled") : "",
                        (t.Lines?.Count ?? 0).ToString()
                    });
                }

                TableFormatter.PrintTable(new[] { "Code", "Name", "Type", "Status", "Lines" }, rows);
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch tariffs: {ex.Message}");
            }
        }
    }
}
