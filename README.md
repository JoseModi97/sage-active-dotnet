# Sage.Active

[![NuGet](https://img.shields.io/nuget/v/Sage.Active.svg)](https://www.nuget.org/packages/Sage.Active)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sage.Active.svg)](https://www.nuget.org/packages/Sage.Active)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-netstandard2.0%20%7C%20net5.0%20%7C%20net6.0%20%7C%20net7.0%20%7C%20net8.0%20%7C%20net9.0-blue.svg)](https://dotnet.microsoft.com/)

An idiomatic, high-performance .NET SDK, ASP.NET Core adapter, and global CLI tool suite for the **Sage Active Public API V2** (GraphQL). Provides complete, zero-config integration for Accounting, General Ledger, Sales Invoicing, Purchase Invoices, Banking, Third Parties, OCR, and Multidimensional Analytics across France, Spain, Germany, and Portugal.

Authored by [Jose Modi](https://github.com/JoseModi97) / [Modi97](https://www.nuget.org/profiles/Modi97).

---

## Highlights

* **Complete API Coverage**: Direct support for all **468+ operations and workflows** in Sage Active V2 (Accounting, Sales, Purchases, Banking, Third Parties, Products, OCR, and Catalog Analytics).
* **Universal .NET Compatibility**: Multi-targeted for `netstandard2.0`, `netcoreapp3.1`, `net5.0`, `net6.0`, `net7.0`, `net8.0`, and `net9.0` (compatible with .NET Framework 4.6.1+, .NET 5+, and all modern .NET releases).
* **Zero-Code Setup CLI**: Companion global tool (`dotnet-sage-active` / `sage-active`) with interactive onboarding wizard, connectivity diagnostics, organization switcher, and project scaffolder.
* **Resilient GraphQL Transport**: Automated OAuth 2.0 SBC Auth token refreshing, rate limit backoff (3,000 req/min), and GraphQL multipart file upload support.
* **First-Class ASP.NET Core**: Dependency injection (`AddSageActive`), Minimal API routes (`MapSageWebhook`), and health check extensions.

---

## Packages

| Package | Description | Target Frameworks |
|---|---|---|
| **`Sage.Active`** | Core engine: Typed GraphQL client, OAuth 2.0 token refresher, error decoder, strong entity models, and domain services | `netstandard2.0`, `netcoreapp3.1`, `net5.0`, `net6.0`, `net7.0`, `net8.0`, `net9.0` |
| **`Sage.Active.AspNetCore`** | ASP.NET Core DI extensions (`AddSageActive`), Minimal API route mapping (`MapSageWebhook`), and webhook handlers | `netstandard2.0`, `netcoreapp3.1`, `net5.0`, `net6.0`, `net7.0`, `net8.0`, `net9.0` |
| **`dotnet-sage-active`** | Global CLI tool (`sage-active`) for interactive setup, diagnostics, sandbox testing, and code scaffolding | `net8.0` (runs on .NET 8, 9, 10+) |

---

## Installation

### Core SDK
```bash
dotnet add package Sage.Active
```

### ASP.NET Core Integration
```bash
dotnet add package Sage.Active.AspNetCore
```

### Global CLI Tool
```bash
dotnet tool install --global dotnet-sage-active
```

---

## CLI Global Tool (`sage-active`)

The `sage-active` CLI tool allows you to configure, test, and operate Sage Active without writing code:

```
   _____                   ___         __  _           
  / ___/____ _____ ____   /   |  _____/ /_(_)   _____  
  \__ \/ __ `/ __ `/ _ \ / /| | / ___/ __/ / | / / _ \ 
 ___/ / /_/ / /_/ /  __// ___ |/ /__/ /_/ /| |/ /  __/ 
/____/\__,_/\__, /\___//_/  |_|\___/\__/_/ |___/\___/  
           /____/                                      
```

### Key CLI Commands

| Command | Description |
|---|---|
| `sage-active init` | Interactive setup wizard (prompts for region, sandbox/prod, credentials, queries organizations, and saves configuration) |
| `sage-active test` | Self-test connectivity, user profile, and validates permissions using `userAccessPolicyCheck` |
| `sage-active org` | Lists available organizations and shows which organization is currently active |
| `sage-active org set <id>` | Switches the active organization ID |
| `sage-active query "<query>"` | Runs an arbitrary GraphQL query or executes a `.graphql` query file |
| `sage-active invoice [list]` | Lists recent sales invoices |
| `sage-active invoice create` | Creates and posts a test sales invoice in Sandbox |
| `sage-active scaffold [minimal\|console]` | Generates ready-to-run starter code in the current directory |

---

## Quickstart: ASP.NET Core Minimal APIs

### 1. Configure Credentials (`appsettings.json`)
```json
{
  "SageActive": {
    "Region": "FR",
    "Environment": "Sandbox",
    "SubscriptionKey": "YOUR_SAGE_SUBSCRIPTION_KEY",
    "OrganizationId": "YOUR_SAGE_ORGANIZATION_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET"
  }
}
```

### 2. Register & Map Endpoints (`Program.cs`)
```csharp
using Sage.Active;
using Sage.Active.AspNetCore;
using Sage.Active.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Register Sage Active SDK via Dependency Injection
builder.Services.AddSageActive(builder.Configuration);

var app = builder.Build();

// Query Chart of Accounts
app.MapGet("/accounting/accounts", async (SageActiveClient client) =>
{
    var accounts = await client.Accounting.GetAccountsAsync();
    return Results.Ok(accounts.Nodes);
});

// Create and Post Sales Invoice in one operation
app.MapPost("/sales/invoice", async (SageActiveClient client, SalesInvoiceCreateInput input) =>
{
    var result = await client.Sales.CreateAndPostInvoiceAsync(input);
    return Results.Ok(new
    {
        invoiceId = result.InvoiceId,
        operationalNumber = result.OperationalNumber,
        accountingEntry = result.AccountingEntryNumber
    });
});

// Receive Webhooks
app.MapSageWebhook("/sage/webhook", async (payload, ctx) =>
{
    Console.WriteLine($"Received Sage Event: {payload}");
    await Task.CompletedTask;
});

app.Run();
```

---

## Quickstart: Standalone / Console Usage

```csharp
using System;
using System.Threading.Tasks;
using Sage.Active;
using Sage.Active.Models;

var config = new SageActiveConfig
{
    Region = SageRegion.FR,
    Environment = SageEnvironment.Sandbox,
    SubscriptionKey = "YOUR_SUBSCRIPTION_KEY",
    OrganizationId = "YOUR_ORGANIZATION_ID",
    AccessToken = "YOUR_BEARER_TOKEN"
};

var client = new SageActiveClient(config);

// 1. Verify User Profile
var profile = await client.Users.GetUserProfileAsync();
Console.WriteLine($"Connected as: {profile.FullName} ({profile.AuthenticationEmail})");

// 2. Query Active Customers
var customers = await client.ThirdParties.GetCustomersAsync(onlyActive: true);
foreach (var customer in customers.Nodes)
{
    Console.WriteLine($"Customer: {customer.Code} - {customer.SocialName}");
}
```

---

## Architecture & Service Coverage

```mermaid
graph LR
    Client[SageActiveClient] --> Org[Organizations & Master Data]
    Client --> Users[Users & Access Policies]
    Client --> Acc[Accounting & General Ledger]
    Client --> TP[Third Parties (CRM)]
    Client --> Prod[Products & Dynamic Pricing]
    Client --> Sales[Sales Documents & Invoicing]
    Client --> Purch[Purchase Invoices & OCR]
    Client --> Bank[Banking & Treasury]
    Client --> Files[Files & Multipart Uploads]
    Client --> Cat[Catalog & Analytics Cubes]
    Client --> Raw[Custom GraphQL Queries]
```

### Complete API Domain Services

* **`client.Organizations`**: Query organizations, onboarding state, legislation codes, countries, zip codes, and currencies.
* **`client.Users`**: Retrieve authenticated user profile, users list, and validate action authorizations via `userAccessPolicyCheck`.
* **`client.Accounting`**: Full chart of accounts, fiscal periods (`accountingExercises`), journal types, tax treatments, balanced GL entries (`createAccountingEntryUsingCodes`), trial balance, balance sheet, and profit & loss.
* **`client.ThirdParties`**: Manage customers, suppliers, employees, multiple addresses, and contacts.
* **`client.Products`**: Product catalog, units of measurement, dynamic price evaluation (`productPriceById`), and sales tariffs.
* **`client.Sales`**: Order-to-cash lifecycle (Quotes $\rightarrow$ Orders $\rightarrow$ Delivery Notes $\rightarrow$ Sales Invoices $\rightarrow$ Credit Notes). Includes automated validation (`closeSalesInvoice`), GL posting (`postSalesInvoice`), and open item settlement (`salesOpenItemSettlement`).
* **`client.Purchases`**: Purchase invoices, AP ledger posting, payables settlement, and automated OCR ingestion.
* **`client.Banks`**: Connected bank accounts, bank statement movements, matching rules, and bank reconciliation (`reconcileBankMovement`).
* **`client.Files`**: GraphQL multipart file attachments to entities, secure download links, and bulk ZIP export.
* **`client.Catalog`**: Multidimensional analytical cubes (`aggregationCatalog`/`aggregationExecute`) and standardized query lists (`listCatalog`/`listExecute`).
* **`client.ExecuteQueryAsync<T>`**: Direct raw GraphQL execution escape hatch for any query, mutation, or schema extension.

---

## Regional Gateways & Sandboxes

Preconfigured endpoints for all European regions:

| Region | Country | Gateway Address | Auth Server |
|---|---|---|---|
| **FR** | France | `https://api.fr.active.sage.com` | `https://sbcauth.sage.fr` |
| **ES** | Spain | `https://api.es.active.sage.com` | `https://sbcauth.sage.fr` |
| **DE** | Germany | `https://api.de.active.sage.com` | `https://sbcauth.sage.fr` |
| **PT** | Portugal | `https://api.es.active.sage.com` | `https://sbcauth.sage.fr` |

---

## Compatibility Matrix

| .NET Version / Platform | Support Mode |
|---|---|
| **.NET 5.0** | **Native Target (`net5.0`) + Fallback (`netstandard2.0`)** |
| **.NET 6.0, 7.0, 8.0, 9.0, 10.0+** | **Native Targets (`net6.0`, `net7.0`, `net8.0`, `net9.0`)** |
| **.NET Core (2.0 – 3.1)** | **Supported via `netcoreapp3.1` and `netstandard2.0`** |
| **.NET Framework (4.6.1 – 4.8.1)** | **Supported via `netstandard2.0`** |
| **Mono / Xamarin / Unity / MAUI** | **Supported via `netstandard2.0`** |

---

## License

MIT © [Jose Modi](https://github.com/JoseModi97) / [Modi97](https://www.nuget.org/profiles/Modi97)
