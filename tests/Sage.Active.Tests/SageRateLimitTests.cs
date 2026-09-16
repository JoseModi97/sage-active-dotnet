using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sage.Active;
using Sage.Active.Exceptions;
using Sage.Active.Models;
using Xunit;

namespace Sage.Active.Tests
{
    public class SageRateLimitTests
    {
        [Fact]
        public async Task SendQuery_When429RateLimited_RetriesAndSucceeds()
        {
            var callCount = 0;

            var mockHandler = new MockHttpMessageHandler((req) =>
            {
                callCount++;
                if (callCount == 1)
                {
                    var msg = new HttpResponseMessage((HttpStatusCode)429)
                    {
                        Content = new StringContent("Too Many Requests", Encoding.UTF8, "text/plain")
                    };
                    msg.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromMilliseconds(50));
                    return msg;
                }

                const string jsonSuccess = @"
                {
                    ""data"": {
                        ""userProfile"": {
                            ""fullName"": ""Jane Doe"",
                            ""authenticationEmail"": ""jane@example.com""
                        }
                    }
                }";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonSuccess, Encoding.UTF8, "application/json")
                };
            });

            var config = new SageActiveConfig
            {
                SubscriptionKey = "sub-key",
                AccessToken = "test-token",
                MaxRetryAttempts = 3
            };

            var client = new SageActiveClient(config, new HttpClient(mockHandler));
            var profile = await client.Users.GetUserProfileAsync();

            Assert.NotNull(profile);
            Assert.Equal("Jane Doe", profile.FullName);
            Assert.Equal(2, callCount);
        }
    }
}
