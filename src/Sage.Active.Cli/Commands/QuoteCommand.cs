using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage.Active.Cli.Commands
{
    public static class QuoteCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            var config = CliConfigLoader.LoadConfig();
            var client = new SageActiveClient(config);

            Prompter.Info("Retrieving sales quotes...");
            try
            {
                var quotes = await client.Sales.GetQuotesAsync(15);
                var rows = new List<List<string>>();

                foreach (var q in quotes.Nodes)
                {
                    rows.Add(new List<string>
                    {
                        q.OperationalNumber ?? "DRAFT",
                        q.SocialName ?? q.CustomerId ?? "",
                        q.DocumentDate.ToString("yyyy-MM-dd"),
                        q.TotalNet.ToString("N2"),
                        q.Status ?? ""
                    });
                }

                TableFormatter.PrintTable(new[] { "Number", "Customer", "Date", "Total Net", "Status" }, rows);
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch quotes: {ex.Message}");
            }
        }
    }
}
