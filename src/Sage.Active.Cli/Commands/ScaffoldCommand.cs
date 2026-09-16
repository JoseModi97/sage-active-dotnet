using System;
using System.IO;

namespace Sage.Active.Cli.Commands
{
    public static class ScaffoldCommand
    {
        public static void Execute(string[] args)
        {
            var template = args.Length > 0 ? args[0].ToLower() : "minimal";

            Prompter.Info($"Scaffolding Sage Active {template} starter code into current directory...");

            if (template == "minimal" || template == "api")
            {
                ScaffoldMinimalApi();
            }
            else
            {
                ScaffoldConsole();
            }

            Prompter.Success("Scaffolding complete! Run 'dotnet run' to start.");
        }

        private static void ScaffoldMinimalApi()
        {
            var code = @"using Sage.Active;
using Sage.Active.AspNetCore;
using Sage.Active.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Register Sage Active SDK
builder.Services.AddSageActive(builder.Configuration);

var app = builder.Build();

app.MapGet(""/sage/organizations"", async (SageActiveClient client) =>
{
    var orgs = await client.Organizations.GetOrganizationsAsync();
    return Results.Ok(orgs.Nodes);
});

app.MapGet(""/sage/invoices"", async (SageActiveClient client) =>
{
    var invoices = await client.Sales.GetInvoicesAsync();
    return Results.Ok(invoices.Nodes);
});

app.MapSageWebhook(""/sage/webhook"", async (payload, ctx) =>
{
    Console.WriteLine($""Received webhook event: {payload}"");
    await Task.CompletedTask;
});

app.Run();";

            File.WriteAllText("Program.cs", code);
            Prompter.Success("Generated Program.cs (ASP.NET Core Minimal API)");
        }

        private static void ScaffoldConsole()
        {
            var code = @"using System;
using System.Threading.Tasks;
using Sage.Active;
using Sage.Active.Models;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new SageActiveConfig
        {
            SubscriptionKey = ""YOUR_SUBSCRIPTION_KEY"",
            OrganizationId = ""YOUR_ORGANIZATION_ID"",
            Region = SageRegion.FR,
            Environment = SageEnvironment.Sandbox
        };

        var client = new SageActiveClient(config);

        Console.WriteLine(""Connecting to Sage Active..."");
        var profile = await client.Users.GetUserProfileAsync();
        Console.WriteLine($""Connected as: {profile.FullName} ({profile.AuthenticationEmail})"");

        var exercises = await client.Accounting.GetExercisesAsync();
        Console.WriteLine($""Exercises count: {exercises.TotalCount}"");
    }
}";

            File.WriteAllText("Program.cs", code);
            Prompter.Success("Generated Program.cs (Standalone Console Script)");
        }
    }
}
