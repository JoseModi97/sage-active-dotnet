using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sage.Active.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC Controllers
builder.Services.AddControllers();

// Register Sage Active SDK via extension
builder.Services.AddSageActive(builder.Configuration.GetSection("SageActive"));

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
