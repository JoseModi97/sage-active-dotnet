using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sage.Active.AspNetCore;
using WorkerServiceExample;

var builder = Host.CreateApplicationBuilder(args);

// Register Sage Active SDK
builder.Services.AddSageActive(builder.Configuration.GetSection("SageActive"));

// Register background polling / sync worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
