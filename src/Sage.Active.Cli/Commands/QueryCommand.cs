using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Sage.Active.Models.GraphQL;

namespace Sage.Active.Cli.Commands
{
    public static class QueryCommand
    {
        public static async Task ExecuteAsync(string[] args)
        {
            if (args.Length == 0)
            {
                Prompter.Warning("Usage: sage-active query \"<graphql-query>\" OR sage-active query <file.graphql>");
                return;
            }

            var queryInput = args[0];
            string query;

            if (File.Exists(queryInput))
            {
                query = File.ReadAllText(queryInput);
                Prompter.Info($"Loaded query from file: {queryInput}");
            }
            else
            {
                query = queryInput;
            }

            var config = CliConfigLoader.LoadConfig();
            var client = new SageActiveClient(config);

            Prompter.Info("Executing GraphQL Query...");
            try
            {
                var request = new GraphQLRequest(query);
                var response = await client.ExecuteRawAsync<JsonElement>(request);

                if (response.HasErrors)
                {
                    Prompter.Error($"Query returned {response.Errors!.Count} error(s):");
                    foreach (var err in response.Errors)
                    {
                        Console.WriteLine($" - {err.Message}");
                    }
                }
                else
                {
                    Prompter.Success("Response Data:");
                    var formatted = JsonSerializer.Serialize(response.Data, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(formatted);
                }
            }
            catch (Exception ex)
            {
                Prompter.Error($"Query failed: {ex.Message}");
            }
        }
    }
}
