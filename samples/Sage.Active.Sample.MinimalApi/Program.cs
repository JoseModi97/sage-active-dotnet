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

// Register Sage Active SDK via DI from appsettings.json section "SageActive"
builder.Services.AddSageActive(builder.Configuration);

var app = builder.Build();

// 1. Root diagnostic endpoint
app.MapGet("/", () => Results.Ok(new
{
    service = "Sage Active .NET Minimal API Sample",
    status = "healthy",
    docs = "/sage/organizations, /sage/invoices, /sage/create-invoice"
}));

// 2. Query available organizations
app.MapGet("/sage/organizations", async (SageActiveClient client) =>
{
    var orgs = await client.Organizations.GetOrganizationsAsync();
    return Results.Ok(orgs.Nodes);
});

// 3. Query sales invoices
app.MapGet("/sage/invoices", async (SageActiveClient client) =>
{
    var invoices = await client.Sales.GetInvoicesAsync(10);
    return Results.Ok(invoices.Nodes);
});

// 4. Quick Sandbox workflow: Create and post a sales invoice in one call
app.MapPost("/sage/create-invoice", async (SageActiveClient client) =>
{
    // Retrieve first customer
    var customers = await client.ThirdParties.GetCustomersAsync(true, 1);
    var customer = customers.Nodes.GetEnumerator();
    if (!customer.MoveNext() || customer.Current == null)
    {
        return Results.BadRequest(new { error = "No active customers found. Create a customer first." });
    }

    // Retrieve first product
    var products = await client.Products.GetProductsAsync(1);
    var product = products.Nodes.GetEnumerator();
    if (!product.MoveNext() || product.Current == null)
    {
        return Results.BadRequest(new { error = "No products found in catalog." });
    }

    var input = new SalesInvoiceCreateInput
    {
        CustomerId = customer.Current.Id,
        DocumentDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"),
        Lines = new List<SalesInvoiceLineInput>
        {
            new SalesInvoiceLineInput
            {
                ProductId = product.Current.Id,
                Quantity = 1,
                Description = "Sample automated sales invoice from Minimal API"
            }
        }
    };

    var result = await client.Sales.CreateAndPostInvoiceAsync(input);

    return Results.Ok(new
    {
        message = "Sales invoice created and posted to general ledger successfully!",
        invoiceId = result.InvoiceId,
        operationalNumber = result.OperationalNumber,
        accountingEntry = result.AccountingEntryNumber
    });
});

// 5. Webhook listener endpoint
app.MapSageWebhook("/sage/webhook", async (payload, ctx) =>
{
    Console.WriteLine($"[Sage Webhook Event Received]: {payload}");
    await System.Threading.Tasks.Task.CompletedTask;
});

app.Run();
