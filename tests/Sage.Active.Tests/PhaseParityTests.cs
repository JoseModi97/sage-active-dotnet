using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active;
using Sage.Active.Models;
using Sage.Active.Models.Entities;
using Xunit;

namespace Sage.Active.Tests
{
    public class PhaseParityTests
    {
        [Fact]
        public async Task WithOrganization_ClonesClientWithTenantHeader()
        {
            HttpRequestMessage? capturedRequest = null;
            var handler = new MockHttpMessageHandler(req =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"{""data"":{""paymentMethods"":{""edges"":[],""totalCount"":0}}}", Encoding.UTF8, "application/json")
                };
            });

            var config = new SageActiveConfig
            {
                SubscriptionKey = "sub-key-1",
                OrganizationId = "original-org",
                AccessToken = "tok-123"
            };

            var originalClient = new SageActiveClient(config, new HttpClient(handler));
            var tenantClient = originalClient.WithOrganization("tenant-branch-888");

            await tenantClient.Banks.GetPaymentMethodsAsync();

            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest!.Headers.Contains("X-OrganizationId"));
            Assert.Equal("tenant-branch-888", capturedRequest.Headers.GetValues("X-OrganizationId").First());
            Assert.Equal("original-org", originalClient.Config.OrganizationId);
            Assert.Equal("tenant-branch-888", tenantClient.Config.OrganizationId);
        }

        [Fact]
        public async Task Sales_GetQuotesAndOrders_ParsesPayloads()
        {
            var handler = new MockHttpMessageHandler(req =>
            {
                var content = req.Content?.ReadAsStringAsync().Result ?? "";
                if (content.Contains("salesQuotes"))
                {
                    const string quoteJson = @"{
                        ""data"": {
                            ""salesQuotes"": {
                                ""edges"": [
                                    {
                                        ""node"": {
                                            ""id"": ""quote-1"",
                                            ""operationalNumber"": ""DEV-2026-001"",
                                            ""documentDate"": ""2026-10-05T00:00:00Z"",
                                            ""totalNet"": 1250.50,
                                            ""status"": ""DRAFT"",
                                            ""lines"": [
                                                {
                                                    ""id"": ""qline-1"",
                                                    ""productCode"": ""SRV01"",
                                                    ""totalQuantity"": 2,
                                                    ""unitPrice"": 625.25,
                                                    ""totalNet"": 1250.50
                                                }
                                            ]
                                        }
                                    }
                                ],
                                ""totalCount"": 1
                            }
                        }
                    }";
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(quoteJson, Encoding.UTF8, "application/json") };
                }
                else
                {
                    const string orderJson = @"{
                        ""data"": {
                            ""salesOrders"": {
                                ""edges"": [
                                    {
                                        ""node"": {
                                            ""id"": ""order-1"",
                                            ""operationalNumber"": ""CMD-2026-001"",
                                            ""documentDate"": ""2026-10-05T00:00:00Z"",
                                            ""totalNet"": 3500.00,
                                            ""status"": ""VALIDATED"",
                                            ""lines"": []
                                        }
                                    }
                                ],
                                ""totalCount"": 1
                            }
                        }
                    }";
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(orderJson, Encoding.UTF8, "application/json") };
                }
            });

            var client = new SageActiveClient(new SageActiveConfig { AccessToken = "tok" }, new HttpClient(handler));

            var quotes = await client.Sales.GetQuotesAsync();
            Assert.NotNull(quotes);
            Assert.Single(quotes.Nodes);
            var quote = quotes.Nodes.First();
            Assert.Equal("quote-1", quote.Id);
            Assert.Equal("DEV-2026-001", quote.OperationalNumber);
            Assert.Single(quote.Lines!);
            Assert.Equal(1250.50m, quote.Lines![0].TotalNet);

            var orders = await client.Sales.GetOrdersAsync();
            Assert.NotNull(orders);
            Assert.Single(orders.Nodes);
            Assert.Equal("CMD-2026-001", orders.Nodes.First().OperationalNumber);
        }

        [Fact]
        public async Task Sales_CreateQuoteAndOrderAndCreditNote_Succeeds()
        {
            var handler = new MockHttpMessageHandler(req =>
            {
                var body = req.Content?.ReadAsStringAsync().Result ?? "";
                if (body.Contains("createSalesQuote"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{""data"":{""createSalesQuote"":{""id"":""quote-uuid-999""}}}", Encoding.UTF8, "application/json")
                    };
                }
                if (body.Contains("createSalesOrder"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{""data"":{""createSalesOrder"":{""id"":""order-uuid-888""}}}", Encoding.UTF8, "application/json")
                    };
                }
                if (body.Contains("generateCreditNote"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{""data"":{""generateCreditNote"":{""id"":""cn-uuid-777""}}}", Encoding.UTF8, "application/json")
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            });

            var client = new SageActiveClient(new SageActiveConfig { AccessToken = "tok" }, new HttpClient(handler));

            var quoteId = await client.Sales.CreateQuoteAsync(new SalesQuoteCreateInput
            {
                CustomerId = "cust-1",
                DocumentDate = "2026-10-05"
            });
            Assert.Equal("quote-uuid-999", quoteId);

            var orderId = await client.Sales.CreateOrderAsync(new SalesOrderCreateInput
            {
                CustomerId = "cust-1",
                DocumentDate = "2026-10-05"
            });
            Assert.Equal("order-uuid-888", orderId);

            var creditNoteId = await client.Sales.GenerateCreditNoteAsync("inv-posted-100");
            Assert.Equal("cn-uuid-777", creditNoteId);
        }

        [Fact]
        public async Task Products_GetTariffs_ParsesCorrectly()
        {
            var handler = new MockHttpMessageHandler(req =>
            {
                const string tariffJson = @"{
                    ""data"": {
                        ""salesTariffs"": {
                            ""edges"": [
                                {
                                    ""node"": {
                                        ""id"": ""tariff-1"",
                                        ""code"": ""TARIFF_VIP"",
                                        ""name"": ""VIP Pricing"",
                                        ""type"": ""DISCOUNT"",
                                        ""enabled"": true,
                                        ""lines"": [
                                            {
                                                ""id"": ""tline-1"",
                                                ""productId"": ""prod-1"",
                                                ""enabled"": true,
                                                ""indicatorValue"": 15.00
                                            }
                                        ]
                                    }
                                }
                            ],
                            ""totalCount"": 1
                        }
                    }
                }";
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tariffJson, Encoding.UTF8, "application/json") };
            });

            var client = new SageActiveClient(new SageActiveConfig { AccessToken = "tok" }, new HttpClient(handler));
            var tariffs = await client.Products.GetTariffsAsync();

            Assert.NotNull(tariffs);
            Assert.Single(tariffs.Nodes);
            var item = tariffs.Nodes.First();
            Assert.Equal("TARIFF_VIP", item.Code);
            Assert.Equal("VIP Pricing", item.Name);
            Assert.True(item.Enabled);
            Assert.Single(item.Lines!);
            Assert.Equal(15.00m, item.Lines![0].IndicatorValue);
        }

        [Fact]
        public async Task Accounting_TaxesAndTerms_ParsesCorrectly()
        {
            var handler = new MockHttpMessageHandler(req =>
            {
                var body = req.Content?.ReadAsStringAsync().Result ?? "";
                if (body.Contains("taxGroups"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{""data"":{""taxGroups"":{""edges"":[{""node"":{""id"":""tg-1"",""name"":""Standard VAT"",""taxGroupCode"":""VAT20""}}],""totalCount"":1}}}", Encoding.UTF8, "application/json")
                    };
                }
                if (body.Contains("taxTreatments"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{""data"":{""taxTreatments"":{""edges"":[{""node"":{""id"":""tt-1"",""code"":""DOMESTIC"",""name"":""France Domestic""}}],""totalCount"":1}}}", Encoding.UTF8, "application/json")
                    };
                }
                if (body.Contains("paymentTerms"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{""data"":{""paymentTerms"":{""edges"":[{""node"":{""id"":""pt-1"",""name"":""30 Days Net"",""lines"":[{""id"":""ptl-1"",""day"":30,""condition"":""NET""}]}}],""totalCount"":1}}}", Encoding.UTF8, "application/json")
                    };
                }
                // taxes
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"{""data"":{""taxes"":{""edges"":[{""node"":{""id"":""tax-1"",""code"":""FR_20"",""name"":""TVA 20%"",""percentage"":20.0}}],""totalCount"":1}}}", Encoding.UTF8, "application/json")
                };
            });

            var client = new SageActiveClient(new SageActiveConfig { AccessToken = "tok" }, new HttpClient(handler));

            var taxes = await client.Accounting.GetTaxesAsync();
            Assert.Single(taxes.Nodes);
            Assert.Equal(20.0m, taxes.Nodes.First().Percentage);

            var groups = await client.Accounting.GetTaxGroupsAsync();
            Assert.Single(groups.Nodes);
            Assert.Equal("VAT20", groups.Nodes.First().TaxGroupCode);

            var treatments = await client.Accounting.GetTaxTreatmentsAsync();
            Assert.Single(treatments.Nodes);
            Assert.Equal("DOMESTIC", treatments.Nodes.First().Code);

            var terms = await client.Accounting.GetPaymentTermsAsync();
            Assert.Single(terms.Nodes);
            Assert.Equal("30 Days Net", terms.Nodes.First().Name);
            Assert.Equal(30, terms.Nodes.First().Lines![0].Day);
        }

        [Fact]
        public async Task PurchasesAndBanks_OperationsSucceed()
        {
            var handler = new MockHttpMessageHandler(req =>
            {
                var body = req.Content?.ReadAsStringAsync().Result ?? "";
                if (body.Contains("unReconcileBankMovement"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{""data"":{""unReconcileBankMovement"":{""id"":""unrec-trans-123""}}}", Encoding.UTF8, "application/json")
                    };
                }
                if (body.Contains("purchaseInvoiceOpenItems"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{""data"":{""purchaseInvoiceOpenItems"":{""edges"":[{""node"":{""id"":""open-1"",""amount"":890.00,""paidAmountAccumulated"":0.00,""status"":""OPEN""}}],""totalCount"":1}}}", Encoding.UTF8, "application/json")
                    };
                }
                if (body.Contains("purchaseInvoices"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{""data"":{""purchaseInvoices"":{""edges"":[{""node"":{""id"":""pinv-1"",""operationalNumber"":""FAC-999"",""uploadFileId"":""file-ocr-1""}}],""totalCount"":1}}}", Encoding.UTF8, "application/json")
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            });

            var client = new SageActiveClient(new SageActiveConfig { AccessToken = "tok" }, new HttpClient(handler));

            var unrecId = await client.Banks.UnreconcileMovementAsync("trans-999");
            Assert.Equal("unrec-trans-123", unrecId);

            var openItems = await client.Purchases.GetOpenItemsAsync();
            Assert.Single(openItems.Nodes);
            Assert.Equal(890.00m, openItems.Nodes.First().Amount);

            var invoices = await client.Purchases.GetInvoicesByFileIdAsync("file-ocr-1");
            Assert.Single(invoices.Nodes);
            Assert.Equal("FAC-999", invoices.Nodes.First().OperationalNumber);
        }
    }
}
