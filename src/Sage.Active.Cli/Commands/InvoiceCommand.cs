using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;

namespace Sage.Active.Cli.Commands
{
    public static class InvoiceCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            var config = CliConfigLoader.LoadConfig();
            var client = new SageActiveClient(config);

            var subCommand = args.Length > 0 ? args[0].ToLower() : "list";

            if (subCommand == "create")
            {
                await CreateTestInvoiceAsync(client);
            }
            else
            {
                await ListInvoicesAsync(client);
            }
        }

        private static async Task ListInvoicesAsync(SageActiveClient client)
        {
            Prompter.Info("Retrieving sales invoices...");
            try
            {
                var invoices = await client.Sales.GetInvoicesAsync(15);
                var rows = new List<List<string>>();

                foreach (var inv in invoices.Nodes)
                {
                    rows.Add(new List<string>
                    {
                        inv.OperationalNumber ?? "DRAFT",
                        inv.Customer?.SocialName ?? inv.CustomerId ?? "",
                        inv.DocumentDate.ToString("yyyy-MM-dd"),
                        inv.TotalLiquid.ToString("C2"),
                        inv.Status ?? ""
                    });
                }

                TableFormatter.PrintTable(new[] { "Number", "Customer", "Date", "Total", "Status" }, rows);
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch invoices: {ex.Message}");
            }
        }

        private static async Task CreateTestInvoiceAsync(SageActiveClient client)
        {
            Prompter.Info("Creating test sales invoice in Sandbox...");
            try
            {
                // 1. Pick first customer
                var customers = await client.ThirdParties.GetCustomersAsync(true, 1);
                var customerId = string.Empty;
                foreach (var c in customers.Nodes)
                {
                    customerId = c.Id;
                    Prompter.Info($"Using Customer: {c.SocialName} ({c.Code})");
                    break;
                }

                if (string.IsNullOrEmpty(customerId))
                {
                    Prompter.Warning("No active customers found. Please create a customer first.");
                    return;
                }

                // 2. Pick first product
                var products = await client.Products.GetProductsAsync(1);
                var productId = string.Empty;
                foreach (var p in products.Nodes)
                {
                    productId = p.Id;
                    Prompter.Info($"Using Product: {p.Name} ({p.Code})");
                    break;
                }

                if (string.IsNullOrEmpty(productId))
                {
                    Prompter.Warning("No products found in catalog.");
                    return;
                }

                var input = new SalesInvoiceCreateInput
                {
                    CustomerId = customerId,
                    DocumentDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"),
                    Lines = new List<SalesInvoiceLineInput>
                    {
                        new SalesInvoiceLineInput
                        {
                            ProductId = productId,
                            Quantity = 1,
                            Description = "Test invoice created via sage-active CLI"
                        }
                    }
                };

                Prompter.Info("Posting invoice to general ledger...");
                var result = await client.Sales.CreateAndPostInvoiceAsync(input);

                Prompter.Success($"Sales Invoice Created & Posted!");
                Prompter.Success($"Invoice ID: {result.InvoiceId}");
                Prompter.Success($"Operational Number: {result.OperationalNumber}");
                Prompter.Success($"Accounting Entry: {result.AccountingEntryNumber}");
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to create invoice: {ex.Message}");
            }
        }
    }
}
