# Examples

Standalone, runnable mini-projects — one per hosting model `sage-active init` auto-detects.
Each references the **real published** `Sage.Active` and `Sage.Active.AspNetCore` packages from NuGet.org (not a project reference to the source in this repo), showing exactly what a consumer's own project looks like.

| Example | Hosting Model | Package Used | How to Run |
|:---|:---|:---|:---|
| [`console-script`](console-script) | Plain Console app, direct instantiation | `Sage.Active` | `cd console-script && dotnet run` |
| [`aspnetcore-minimal-api`](aspnetcore-minimal-api) | ASP.NET Core Minimal API with DI & Webhook mapping | `Sage.Active.AspNetCore` | `cd aspnetcore-minimal-api && dotnet run` |
| [`aspnetcore-mvc`](aspnetcore-mvc) | ASP.NET Core MVC with Controllers | `Sage.Active.AspNetCore` | `cd aspnetcore-mvc && dotnet run` |
| [`worker-service`](worker-service) | .NET Generic Host / `BackgroundService` | `Sage.Active.AspNetCore` | `cd worker-service && dotnet run` |
| [`file-based-app`](file-based-app) | .NET 10 File-Based App, zero `.csproj` | `Sage.Active` | `dotnet run file-based-app/app.cs` |

---

## 🔒 Secret Safety & Sandbox Credentials

To run and experiment with these examples against the live Sage Active Sandbox environment:

1. **Provide your own Sandbox credentials** from your [Sage Developer Portal](https://developer.sage.com) account:
   - `ClientId`
   - `ClientSecret`
   - `SubscriptionKey` (or `ApiKey`)
   - `OrganizationId` / `CompanyId`
2. **Never commit real credentials**:
   - Every example uses placeholder values (`"YOUR_SANDBOX_CLIENT_ID"`, etc.) by default.
   - You can pass credentials securely via **Environment Variables** or `.NET User Secrets`:
     ```bash
     # Using Environment Variables (supported by all examples)
     export SageActive__ClientId="your-sandbox-client-id"
     export SageActive__ClientSecret="your-sandbox-client-secret"
     export SageActive__SubscriptionKey="your-subscription-key"
     export SageActive__OrganizationId="your-sandbox-company-id"
     export SageActive__Environment="Sandbox"
     ```
     Or on Windows PowerShell:
     ```powershell
     $env:SageActive__ClientId = "your-sandbox-client-id"
     $env:SageActive__ClientSecret = "your-sandbox-client-secret"
     $env:SageActive__SubscriptionKey = "your-subscription-key"
     $env:SageActive__OrganizationId = "your-sandbox-company-id"
     $env:SageActive__Environment = "Sandbox"
     ```
   - For ASP.NET Core and Worker Service examples, you can also use **.NET Secret Manager**:
     ```bash
     dotnet user-secrets set "SageActive:ClientId" "your-sandbox-client-id"
     dotnet user-secrets set "SageActive:ClientSecret" "your-sandbox-client-secret"
     dotnet user-secrets set "SageActive:SubscriptionKey" "your-subscription-key"
     dotnet user-secrets set "SageActive:OrganizationId" "your-sandbox-company-id"
     dotnet user-secrets set "SageActive:Environment" "Sandbox"
     ```
   - All secret files (`appsettings.Development.json`, `appsettings.local.json`, `*.env`, and user secrets) are explicitly ignored by `.gitignore`.

---

## What Each Example Demonstrates

### 1. [`console-script`](console-script)
- Directly instantiates `SageActiveClient` with `SageActiveConfig` set to `SageEnvironment.Sandbox`.
- Reads credentials from environment variables or safe interactive prompt.
- Queries organizations, retrieves chart of accounts, and tests customer listing.

### 2. [`aspnetcore-minimal-api`](aspnetcore-minimal-api)
- Demonstrates `builder.Services.AddSageActive(builder.Configuration.GetSection("SageActive"))`.
- Exposes REST endpoints:
  - `GET /api/sage/ping`: Connection & authentication health probe
  - `GET /api/sage/customers`: Fetches customer list
  - `GET /api/sage/invoices`: Queries sales invoices
  - `POST /api/sage/invoices`: Creates and posts a test sandbox sales invoice
  - `POST /api/webhooks/sage`: Automated webhook endpoint using `app.MapSageWebhook(...)`

### 3. [`aspnetcore-mvc`](aspnetcore-mvc)
- Demonstrates standard ASP.NET Core MVC controller pattern injecting `SageActiveClient`.
- Contains `InvoicesController` with Actions for listing invoices, viewing details, and receiving webhooks with `SageActiveEndpointExtensions.ReadWebhookBodyAsync(Request)`.

### 4. [`worker-service`](worker-service)
- Demonstrates automated background worker pattern (`BackgroundService`).
- Injects `SageActiveClient` to perform periodic scheduled sync tasks (e.g. checking pending OCR documents or pulling recently modified ledger transactions).

### 5. [`file-based-app`](file-based-app)
- Demonstrates .NET 10 top-level file-based execution with `#:package Sage.Active@1.0.0`.
- Zero `.csproj` required — single self-contained script file runnable with `dotnet run app.cs`.
