using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
#endif

namespace Sage.Active.AspNetCore
{
    public static class SageActiveEndpointExtensions
    {
#if NET6_0_OR_GREATER
        /// <summary>
        /// Maps an HTTP POST endpoint for receiving Sage Active event notifications / webhooks with automated payload parsing.
        /// </summary>
        public static IEndpointRouteBuilder MapSageWebhook(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            Func<string, HttpContext, Task> onEventReceived)
        {
            endpoints.MapPost(pattern, async (HttpContext context) =>
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
                var body = await reader.ReadToEndAsync();

                if (onEventReceived != null)
                {
                    await onEventReceived(body, context);
                }

                return Results.Ok(new { status = "received" });
            });

            return endpoints;
        }
#endif

        /// <summary>
        /// Helper for MVC controllers and handlers to read raw webhook body.
        /// </summary>
        public static async Task<string> ReadWebhookBodyAsync(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }
}
