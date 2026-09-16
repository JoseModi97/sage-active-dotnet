using System;
using System.Threading.Tasks;
using Sage.Active;
using Sage.Active.Models;

namespace Sage.Active.Sample.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("      Sage Active .NET SDK - Console Demo        ");
            Console.WriteLine("=================================================");

            var config = new SageActiveConfig
            {
                Region = SageRegion.FR,
                Environment = SageEnvironment.Sandbox,
                SubscriptionKey = "YOUR_SUBSCRIPTION_KEY",
                OrganizationId = "YOUR_ORGANIZATION_ID",
                AccessToken = "YOUR_BEARER_TOKEN"
            };

            var client = new SageActiveClient(config);

            try
            {
                Console.WriteLine("\n1. Querying User Profile...");
                var profile = await client.Users.GetUserProfileAsync();
                Console.WriteLine($"Connected as: {profile.FullName} ({profile.AuthenticationEmail})");

                Console.WriteLine("\n2. Querying Organizations...");
                var orgs = await client.Organizations.GetOrganizationsAsync();
                Console.WriteLine($"Organizations found: {orgs.TotalCount}");

                Console.WriteLine("\n3. Querying Chart of Accounts...");
                var accounts = await client.Accounting.GetAccountsAsync(5);
                foreach (var acc in accounts.Nodes)
                {
                    Console.WriteLine($" - Account: {acc.Code} ({acc.Name})");
                }

                Console.WriteLine("\n4. Querying Products...");
                var products = await client.Products.GetProductsAsync(5);
                foreach (var prod in products.Nodes)
                {
                    Console.WriteLine($" - Product: {prod.Code} ({prod.Name})");
                }

                Console.WriteLine("\nDemo completed successfully!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError executing Sage Active SDK call: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
