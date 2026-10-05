using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage.Active.Cli.Commands
{
    public static class BankCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            var config = CliConfigLoader.LoadConfig();
            var client = new SageActiveClient(config);

            var subCommand = args.Length > 0 ? args[0].ToLower() : "accounts";

            if (subCommand == "movements")
            {
                await ListMovementsAsync(client);
            }
            else if (subCommand == "unreconcile" && args.Length > 1)
            {
                await UnreconcileAsync(client, args[1]);
            }
            else
            {
                await ListAccountsAsync(client);
            }
        }

        private static async Task ListAccountsAsync(SageActiveClient client)
        {
            Prompter.Info("Retrieving bank accounts...");
            try
            {
                var accounts = await client.Banks.GetBankAccountsAsync(20);
                var rows = new List<List<string>>();

                foreach (var a in accounts.Nodes)
                {
                    rows.Add(new List<string>
                    {
                        a.Id,
                        a.BankName ?? "",
                        a.Iban ?? "",
                        a.Bic ?? ""
                    });
                }

                TableFormatter.PrintTable(new[] { "ID", "Bank Name", "IBAN", "BIC" }, rows);
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch bank accounts: {ex.Message}");
            }
        }

        private static async Task ListMovementsAsync(SageActiveClient client)
        {
            Prompter.Info("Retrieving bank movements...");
            try
            {
                var movements = await client.Banks.GetBankMovementsAsync(first: 20);
                var rows = new List<List<string>>();

                foreach (var m in movements.Nodes)
                {
                    rows.Add(new List<string>
                    {
                        m.Id,
                        m.Date.ToString("yyyy-MM-dd"),
                        m.Description ?? "",
                        m.Amount.ToString("N2"),
                        m.IsReconciled ? "Yes" : "No"
                    });
                }

                TableFormatter.PrintTable(new[] { "ID", "Date", "Description", "Amount", "Reconciled" }, rows);
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to fetch bank movements: {ex.Message}");
            }
        }

        private static async Task UnreconcileAsync(SageActiveClient client, string transactionId)
        {
            Prompter.Info($"Unreconciling bank movement: {transactionId}...");
            try
            {
                var res = await client.Banks.UnreconcileMovementAsync(transactionId);
                Prompter.Success($"Bank movement successfully unreconciled! ID: {res}");
            }
            catch (Exception ex)
            {
                Prompter.Error($"Failed to unreconcile movement: {ex.Message}");
            }
        }
    }
}
