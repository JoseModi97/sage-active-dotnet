using System;
using System.Linq;
using System.Threading.Tasks;
using Sage.Active;
using Sage.Active.Models;

Console.WriteLine("==================================================");
Console.WriteLine(" Sage Active SDK - Standalone Console Script Demo ");
Console.WriteLine("==================================================");

// Load credentials safely from Environment variables or fallback to placeholders
var clientId = Environment.GetEnvironmentVariable("SageActive__ClientId") ?? "YOUR_SANDBOX_CLIENT_ID";
var clientSecret = Environment.GetEnvironmentVariable("SageActive__ClientSecret") ?? "YOUR_SANDBOX_CLIENT_SECRET";
var subscriptionKey = Environment.GetEnvironmentVariable("SageActive__SubscriptionKey") ?? "YOUR_SANDBOX_SUBSCRIPTION_KEY";
var orgId = Environment.GetEnvironmentVariable("SageActive__OrganizationId") ?? "YOUR_SANDBOX_ORG_ID";

if (clientId == "YOUR_SANDBOX_CLIENT_ID" || clientSecret == "YOUR_SANDBOX_CLIENT_SECRET")
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n[NOTE] Running with placeholder Sandbox credentials.");
    Console.WriteLine("To talk to the live Sage Active Sandbox, set environment variables:");
    Console.WriteLine("  $env:SageActive__ClientId = 'your-client-id'");
    Console.WriteLine("  $env:SageActive__ClientSecret = 'your-client-secret'");
    Console.WriteLine("  $env:SageActive__SubscriptionKey = 'your-subscription-key'");
    Console.WriteLine("  $env:SageActive__OrganizationId = 'your-org-id'\n");
    Console.ResetColor();
}

// 1. Initialize client configured explicitly for Sandbox
var config = new SageActiveConfig
{
    Environment = SageEnvironment.Sandbox, // Uses https://sandbox-active.sage.com/api/v2
    Region = SageRegion.FR,
    ClientId = clientId,
    ClientSecret = clientSecret,
    SubscriptionKey = subscriptionKey,
    OrganizationId = orgId
};

Console.WriteLine($"Initialized Sage Active Client for Environment: {config.Environment}");
Console.WriteLine($"Base GraphQL Endpoint: {config.GetEffectiveBaseAddress()}");

var client = new SageActiveClient(config);

try
{
    Console.WriteLine("\n[1/3] Querying Organizations...");
    var orgs = await client.Organizations.GetOrganizationsAsync();
    Console.WriteLine($"Found {orgs.TotalCount} organization(s).");
    foreach (var org in orgs.Nodes)
    {
        Console.WriteLine($" - [{org.Id}] {org.SocialName} (Legislation: {org.LegislationCode})");
    }

    Console.WriteLine("\n[2/3] Querying Customers (ThirdParties)...");
    var customers = await client.ThirdParties.GetCustomersAsync(onlyActive: true, first: 5);
    Console.WriteLine($"Retrieved {customers.Nodes.Count()} customer(s).");
    foreach (var c in customers.Nodes)
    {
        Console.WriteLine($" - [{c.Id}] {c.SocialName} (Code: {c.Code})");
    }

    Console.WriteLine("\n[3/3] Querying Sales Invoices...");
    var invoices = await client.Sales.GetInvoicesAsync(first: 5);
    Console.WriteLine($"Retrieved {invoices.Nodes.Count()} invoice(s).");
    foreach (var inv in invoices.Nodes)
    {
        Console.WriteLine($" - [{inv.Id}] {inv.OperationalNumber} ({inv.DocumentDate}) - Total: {inv.TotalNet}");
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n[Result]: Operation handled gracefully. Detail: {ex.Message}");
    Console.ResetColor();
    Console.WriteLine("(Provide valid Sandbox credentials to execute live queries against Sage Business Cloud).");
}

Console.WriteLine("\nConsole Script sample completed successfully.");
