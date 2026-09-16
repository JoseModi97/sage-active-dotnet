#:package Sage.Active@1.0.0
#:property PublishTrimmed=false
#:property JsonSerializerIsReflectionEnabledByDefault=true

// .NET 10 file-based app usage of Sage.Active - run directly with `dotnet run app.cs`,
// no .csproj required.
//
// To experiment against the live Sage Active Sandbox, supply your credentials via environment variables:
//   $env:SageActive__ClientId = "your-sandbox-client-id"
//   $env:SageActive__ClientSecret = "your-sandbox-client-secret"
//   $env:SageActive__SubscriptionKey = "your-subscription-key"
//   $env:SageActive__OrganizationId = "your-org-id"

using System;
using Sage.Active;
using Sage.Active.Models;

Console.WriteLine("=================================================");
Console.WriteLine(" Sage Active .NET 10 File-Based App (Single File)");
Console.WriteLine("=================================================");

var clientId = Environment.GetEnvironmentVariable("SageActive__ClientId") ?? "YOUR_SANDBOX_CLIENT_ID";
var clientSecret = Environment.GetEnvironmentVariable("SageActive__ClientSecret") ?? "YOUR_SANDBOX_CLIENT_SECRET";
var subscriptionKey = Environment.GetEnvironmentVariable("SageActive__SubscriptionKey") ?? "YOUR_SANDBOX_SUBSCRIPTION_KEY";
var orgId = Environment.GetEnvironmentVariable("SageActive__OrganizationId") ?? "YOUR_SANDBOX_ORG_ID";

var config = new SageActiveConfig
{
    Environment = SageEnvironment.Sandbox, // Points directly to https://sandbox-active.sage.com/api/v2
    Region = SageRegion.FR,
    ClientId = clientId,
    ClientSecret = clientSecret,
    SubscriptionKey = subscriptionKey,
    OrganizationId = orgId
};

var client = new SageActiveClient(config);

Console.WriteLine($"Target Environment : {config.Environment}");
Console.WriteLine($"Base Endpoint      : {config.GetEffectiveBaseAddress()}");

try
{
    Console.WriteLine("\nFetching organizations from Sandbox...");
    var orgs = await client.Organizations.GetOrganizationsAsync();
    Console.WriteLine($"Discovered {orgs.TotalCount} organization(s).");
    foreach (var org in orgs.Nodes)
    {
        Console.WriteLine($" - [{org.Id}] {org.SocialName}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"API Call result: {ex.Message}");
    Console.WriteLine("Provide real Sandbox credentials via environment variables to query live SBC Sandbox entities.");
}

Console.WriteLine("\nExecution finished.");
