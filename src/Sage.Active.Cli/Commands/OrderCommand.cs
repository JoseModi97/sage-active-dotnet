using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage.Active.Cli.Commands
{
    public static class OrderCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            var config = CliConfigLoader.LoadConfig();
            var client = new SageActiveClient(config);

            Prompter.Info("Retrieving sales orders...");
            try
            {
                var orders = await client.Sales.GetOrdersAsync(15);
                var rows = new List<List<string>>();

                foreach (var o in orders.Nodes)
                {
                    rows.Add(new List<string>
                    {
                        o.OperationalNumber ?? "DRAFT",
                        o.SocialName ?? o.CustomerId ?? "",
                        o.DocumentDate.ToString("yyyy-MM-dd"),
                        o.TotalNet.ToString("N2"),
                        o.Status ?? ""
                    });
                }

                TableFormatter.PrintTable(new[] { "Number", "Customer", "Date", "Total Net", "Status" }, rows);
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch orders: {ex.Message}");
            }
        }
    }
}
