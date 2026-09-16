using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sage.Active;
using Sage.Active.AspNetCore;
using Sage.Active.Models.Entities;

namespace AspNetCoreMvcExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly SageActiveClient _client;

        public InvoicesController(SageActiveClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices([FromQuery] int limit = 10)
        {
            try
            {
                var invoices = await _client.Sales.GetInvoicesAsync(limit);
                return Ok(invoices.Nodes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}/open-items")]
        public async Task<IActionResult> GetInvoiceOpenItems(string id)
        {
            try
            {
                var openItems = await _client.Sales.GetOpenItemsAsync(id);
                return Ok(openItems.Nodes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] SalesInvoiceCreateInput input)
        {
            try
            {
                var result = await _client.Sales.CreateAndPostInvoiceAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            // Read raw body using SageActiveEndpointExtensions helper
            var body = await SageActiveEndpointExtensions.ReadWebhookBodyAsync(Request);
            Console.WriteLine($"[MVC Webhook Received]: {body}");
            return Ok(new { status = "received" });
        }
    }
}
