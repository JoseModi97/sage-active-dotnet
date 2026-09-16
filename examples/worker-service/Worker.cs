using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sage.Active;

namespace WorkerServiceExample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SageActiveClient _client;

        public Worker(ILogger<Worker> logger, SageActiveClient client)
        {
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sage Active Background Worker running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("[Sync Task] Polling Sage Active Sandbox for latest invoices and ledger status...");
                    
                    var invoices = await _client.Sales.GetInvoicesAsync(first: 3, cancellationToken: stoppingToken);
                    var count = invoices.Nodes.Count();
                    _logger.LogInformation("Successfully connected to Sage Active. Found {Count} invoice(s).", count);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Worker polled Sage Active: {Message} (Provide Sandbox credentials to sync live records)", ex.Message);
                }

                // Poll every 30 seconds (or on schedule)
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
