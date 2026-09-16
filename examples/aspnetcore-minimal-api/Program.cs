using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sage.Active;
using Sage.Active.AspNetCore;
using Sage.Active.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Register Sage Active SDK using ASP.NET Core extension
builder.Services.AddSageActive(builder.Configuration.GetSection("SageActive"));

var app = builder.Build();

// 1. Root Service Info
app.MapGet("/", () => Results.Ok(new
{
    service = "Sage Active .NET Minimal API Example",
    environment = builder.Configuration["SageActive:Environment"] ?? "Sandbox",
    endpoints = new[]
    {
        "GET  /api/sage/organizations",
        "GET  /api/sage/customers",
        "GET  /api/sage/invoices",
        "POST /api/sage/create-invoice",
        "POST /api/webhooks/sage"
    },
    note = "Set your Sandbox credentials via environment variables or dotnet user-secrets."
}));

// 2. Query Organizations
app.MapGet("/api/sage/organizations", async (SageActiveClient client) =>
{
    try
    {
        var orgs = await client.Organizations.GetOrganizationsAsync();
        return Results.Ok(orgs.Nodes);
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Sage API Error", detail: ex.Message);
    }
});

// 3. Query Customers
app.MapGet("/api/sage/customers", async (SageActiveClient client) =>
{
    try
    {
        var customers = await client.ThirdParties.GetCustomersAsync(onlyActive: true, first: 10);
        return Results.Ok(customers.Nodes);
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Sage API Error", detail: ex.Message);
    }
});

// 4. Query Invoices
app.MapGet("/api/sage/invoices", async (SageActiveClient client) =>
{
    try
    {
        var invoices = await client.Sales.GetInvoicesAsync(first: 10);
        return Results.Ok(invoices.Nodes);
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Sage API Error", detail: ex.Message);
    }
});

// 5. Create & Post Sales Invoice in Sandbox
app.MapPost("/api/sage/create-invoice", async (SageActiveClient client) =>
{
    try
    {
        var customers = await client.ThirdParties.GetCustomersAsync(first: 1);
        var enumerator = customers.Nodes.GetEnumerator();
        if (!enumerator.MoveNext() || enumerator.Current == null)
        {
            return Results.BadRequest(new { error = "No customer found in the sandbox organization." });
        }

        var input = new SalesInvoiceCreateInput
        {
            CustomerId = enumerator.Current.Id,
            DocumentDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"),
            Lines = new List<SalesInvoiceLineInput>
            {
                new SalesInvoiceLineInput
                {
                    Description = "Sandbox automated consultation service",
                    Quantity = 1
                }
            }
        };

        var result = await client.Sales.CreateAndPostInvoiceAsync(input);
        return Results.Ok(new
        {
            message = "Sales invoice created and posted successfully in Sage Sandbox!",
            invoiceId = result.InvoiceId,
            operationalNumber = result.OperationalNumber
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Invoice Creation Error", detail: ex.Message);
    }
});

// 6. Map Sage Webhook listener using Sage.Active.AspNetCore extension
app.MapSageWebhook("/api/webhooks/sage", async (payload, context) =>
{
    Console.WriteLine($"[Sage Webhook Event Received]: {payload}");
    await System.Threading.Tasks.Task.CompletedTask;
});

app.Run();
