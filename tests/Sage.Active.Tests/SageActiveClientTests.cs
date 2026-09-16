using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active;
using Sage.Active.Exceptions;
using Sage.Active.Models;
using Xunit;

namespace Sage.Active.Tests
{
    public class SageActiveClientTests
    {
        [Fact]
        public void Config_EffectiveBaseAddress_ResolvesCorrectly()
        {
            var configFR = new SageActiveConfig { Region = SageRegion.FR };
            Assert.Equal("https://api.fr.active.sage.com", configFR.GetEffectiveBaseAddress());

            var configES = new SageActiveConfig { Region = SageRegion.ES };
            Assert.Equal("https://api.es.active.sage.com", configES.GetEffectiveBaseAddress());

            var configDE = new SageActiveConfig { Region = SageRegion.DE };
            Assert.Equal("https://api.de.active.sage.com", configDE.GetEffectiveBaseAddress());

            var configCustom = new SageActiveConfig { BaseAddress = "https://custom.sage.internal/graphql" };
            Assert.Equal("https://custom.sage.internal", configCustom.GetEffectiveBaseAddress());
        }

        [Fact]
        public async Task GetOrganizations_InjectsRequiredHeaders()
        {
            HttpRequestMessage? capturedRequest = null;

            var mockHandler = new MockHttpMessageHandler((req) =>
            {
                capturedRequest = req;
                const string jsonResponse = @"
                {
                    ""data"": {
                        ""organizations"": {
                            ""edges"": [
                                {
                                    ""node"": {
                                        ""id"": ""00000000-0000-0000-0000-000000000001"",
                                        ""socialName"": ""Test Org"",
                                        ""legislationCode"": ""FR"",
                                        ""status"": ""READY"",
                                        ""onboardingCompleted"": true
                                    }
                                }
                            ],
                            ""totalCount"": 1,
                            ""pageInfo"": { ""hasNextPage"": false }
                        }
                    }
                }";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                };
            });

            var config = new SageActiveConfig
            {
                SubscriptionKey = "sub-test-key-123",
                OrganizationId = "org-test-uuid-456",
                AccessToken = "static-test-token"
            };

            var httpClient = new HttpClient(mockHandler);
            var client = new SageActiveClient(config, httpClient);

            var orgs = await client.Organizations.GetOrganizationsAsync();

            Assert.NotNull(orgs);
            Assert.Equal(1, orgs.TotalCount);
            var list = orgs.Nodes.ToList();
            Assert.Single(list);
            Assert.Equal("Test Org", list[0].SocialName);

            // Verify headers
            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest!.Headers.Contains("x-api-key"));
            Assert.Equal("sub-test-key-123", capturedRequest.Headers.GetValues("x-api-key").First());

            Assert.True(capturedRequest.Headers.Contains("X-OrganizationId"));
            Assert.Equal("org-test-uuid-456", capturedRequest.Headers.GetValues("X-OrganizationId").First());

            Assert.NotNull(capturedRequest.Headers.Authorization);
            Assert.Equal("Bearer", capturedRequest.Headers.Authorization!.Scheme);
            Assert.Equal("static-test-token", capturedRequest.Headers.Authorization.Parameter);
        }

        [Fact]
        public async Task ExecuteQuery_WhenGraphQLErrors_ThrowsSageApiException()
        {
            var mockHandler = new MockHttpMessageHandler((req) =>
            {
                const string jsonResponse = @"
                {
                    ""errors"": [
                        {
                            ""message"": ""The field 'unknownField' does not exist on type 'Query'."",
                            ""extensions"": { ""code"": ""HC0026"" }
                        }
                    ]
                }";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                };
            });

            var config = new SageActiveConfig
            {
                SubscriptionKey = "sub-key",
                AccessToken = "token"
            };

            var client = new SageActiveClient(config, new HttpClient(mockHandler));

            var ex = await Assert.ThrowsAsync<SageApiException>(() => client.ExecuteQueryAsync<object>("{ unknownField }"));
            Assert.Contains("unknownField", ex.Message);
            Assert.NotNull(ex.Errors);
            Assert.Single(ex.Errors!);
            Assert.Equal("HC0026", ex.Errors![0].Code);
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}
