using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage.Active.Cli.Commands
{
    public static class TaxCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            var config = CliConfigLoader.LoadConfig();
            var client = new SageActiveClient(config);

            var subCommand = args.Length > 0 ? args[0].ToLower() : "list";

            if (subCommand == "groups")
            {
                await ListTaxGroupsAsync(client);
            }
            else if (subCommand == "terms")
            {
                await ListPaymentTermsAsync(client);
            }
            else
            {
                await ListTaxesAsync(client);
            }
        }

        private static async Task ListTaxesAsync(SageActiveClient client)
        {
            Prompter.Info("Retrieving fiscal taxes...");
            try
            {
                var taxes = await client.Accounting.GetTaxesAsync(20);
                var rows = new List<List<string>>();

                foreach (var t in taxes.Nodes)
                {
                    rows.Add(new List<string>
                    {
                        t.Code,
                        t.Name ?? "",
                        t.Percentage.HasValue ? $"{t.Percentage.Value:N2}%" : (t.Rate.HasValue ? $"{t.Rate.Value:N2}%" : "0%"),
                        t.GroupName ?? "",
                        t.TaxType ?? ""
                    });
                }

                TableFormatter.PrintTable(new[] { "Code", "Name", "Rate", "Group", "Type" }, rows);
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch taxes: {ex.Message}");
            }
        }

        private static async Task ListTaxGroupsAsync(SageActiveClient client)
        {
            Prompter.Info("Retrieving tax groups...");
            try
            {
                var groups = await client.Accounting.GetTaxGroupsAsync(20);
                var rows = new List<List<string>>();

                foreach (var g in groups.Nodes)
                {
                    rows.Add(new List<string>
                    {
                        g.Id,
                        g.TaxGroupCode ?? "",
                        g.Name ?? "",
                        g.TaxType ?? ""
                    });
                }

                TableFormatter.PrintTable(new[] { "ID", "Code", "Name", "Type" }, rows);
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch tax groups: {ex.Message}");
            }
        }

        private static async Task ListPaymentTermsAsync(SageActiveClient client)
        {
            Prompter.Info("Retrieving commercial payment terms...");
            try
            {
                var terms = await client.Accounting.GetPaymentTermsAsync(20);
                var rows = new List<List<string>>();

                foreach (var pt in terms.Nodes)
                {
                    rows.Add(new List<string>
                    {
                        pt.Id,
                        pt.Name ?? "",
                        (pt.Lines?.Count ?? 0).ToString()
                    });
                }

                TableFormatter.PrintTable(new[] { "ID", "Name", "Lines" }, rows);
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch payment terms: {ex.Message}");
            }
        }
    }
}
