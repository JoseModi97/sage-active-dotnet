using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Exceptions;
using Sage.Active.Models.Entities;
using Sage.Active.Models.GraphQL;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class SalesClient
    {
        private readonly SageGraphQLClient _transport;

        public SalesClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves sales invoices with optional status filter.
        /// </summary>
        public async Task<Connection<SalesInvoice>> GetInvoicesAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                salesInvoices(order: [{ documentDate: DESC }], first: $first) {
                    edges {
                        node {
                            id
                            operationalNumber
                            documentDate
                            firstDueDate
                            status
                            totalNet
                            totalLiquid
                            customer {
                                id
                                code
                                socialName
                            }
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<SalesInvoicesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.SalesInvoices;
        }

        /// <summary>
        /// Creates a draft sales invoice.
        /// </summary>
        public async Task<string> CreateInvoiceAsync(SalesInvoiceCreateInput input, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($values: SalesInvoiceCreateGLDtoInput!) {
                createSalesInvoice(input: $values) {
                    id
                }
            }";

            var variables = new { values = input };
            var result = await _transport.SendQueryAsync<CreateInvoiceResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.CreateSalesInvoice.Id;
        }

        /// <summary>
        /// Validates a draft sales invoice and assigns its definitive operational number.
        /// </summary>
        public async Task<CloseSalesInvoiceResult> CloseInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($input: CloseSalesInvoiceGLDtoInput!) {
                closeSalesInvoice(input: $input) {
                    id
                    operationalNumber
                }
            }";

            var variables = new { input = new { id = invoiceId } };
            var result = await _transport.SendQueryAsync<CloseInvoiceResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.CloseSalesInvoice;
        }

        /// <summary>
        /// Posts a validated sales invoice to the General Ledger, producing an accounting entry.
        /// </summary>
        public async Task<PostSalesInvoiceResult> PostInvoiceAsync(string invoiceId, string? journalTypeId = null, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($input: PostSalesInvoiceGLDtoInput!) {
                postSalesInvoice(input: $input) {
                    accountingEntryId
                    accountingEntryNumber
                }
            }";

            var variables = new { input = new { id = invoiceId, journalTypeId } };
            var result = await _transport.SendQueryAsync<PostInvoiceResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.PostSalesInvoice;
        }

        /// <summary>
        /// Retrieves open receivables (due dates) for a sales invoice.
        /// </summary>
        public async Task<Connection<SalesInvoiceOpenItem>> GetOpenItemsAsync(string invoiceId, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($Id: UUID!) {
                salesInvoiceOpenItems(
                    order: { dueDate: ASC }
                    where: { salesInvoice: { id: { eq: $Id } } }
                ) {
                    edges {
                        node {
                            id
                            dueDate
                            amount
                            paidAmountAccumulated
                            status
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { Id = invoiceId };
            var result = await _transport.SendQueryAsync<OpenItemsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.SalesInvoiceOpenItems;
        }

        /// <summary>
        /// Records payment settlement against an open item.
        /// </summary>
        public async Task<SalesOpenItemSettlementResult> SettleOpenItemAsync(SalesOpenItemSettlementInput input, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($input: SalesOpenItemSettlementGLDtoInput!) {
                salesOpenItemSettlement(input: $input) {
                    accountingEntryId
                    accountingEntryNumber
                }
            }";

            var variables = new { input };
            var result = await _transport.SendQueryAsync<SettlementResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.SalesOpenItemSettlement;
        }

        /// <summary>
        /// High-level convenience method: Creates a draft invoice, validates/numbers it, and posts it to the general ledger in one operation.
        /// </summary>
        public async Task<(string InvoiceId, string OperationalNumber, string? AccountingEntryNumber)> CreateAndPostInvoiceAsync(SalesInvoiceCreateInput input, CancellationToken cancellationToken = default)
        {
            // 1. Create draft invoice
            var invoiceId = await CreateInvoiceAsync(input, cancellationToken).ConfigureAwait(false);

            // 2. Validate and number invoice
            var closed = await CloseInvoiceAsync(invoiceId, cancellationToken).ConfigureAwait(false);

            // 3. Post to GL
            var posted = await PostInvoiceAsync(invoiceId, cancellationToken: cancellationToken).ConfigureAwait(false);

            return (invoiceId, closed.OperationalNumber, posted.AccountingEntryNumber);
        }

        /// <summary>
        /// Retrieves commercial sales quotes (devis).
        /// </summary>
        public async Task<Connection<SalesQuote>> GetQuotesAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                salesQuotes(order: [{ documentDate: DESC }], first: $first) {
                    edges {
                        node {
                            id
                            operationalNumber
                            documentDate
                            creationDate
                            customerId
                            socialName
                            status
                            declinedReason
                            totalNet
                            discount
                            documentTypeId
                            lines {
                                id
                                order
                                productId
                                productCode
                                productName
                                totalQuantity
                                unitPrice
                                firstDiscount
                                totalNet
                            }
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<SalesQuotesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.SalesQuotes;
        }

        /// <summary>
        /// Creates a commercial sales quote.
        /// </summary>
        public async Task<string> CreateQuoteAsync(SalesQuoteCreateInput input, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($values: SalesQuoteCreateGLDtoInput!) {
                createSalesQuote(input: $values) {
                    id
                }
            }";

            var variables = new { values = input };
            var result = await _transport.SendQueryAsync<CreateQuoteResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.CreateSalesQuote.Id;
        }

        /// <summary>
        /// Retrieves sales orders (commandes de vente).
        /// </summary>
        public async Task<Connection<SalesOrder>> GetOrdersAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                salesOrders(order: [{ documentDate: DESC }], first: $first) {
                    edges {
                        node {
                            id
                            operationalNumber
                            documentDate
                            creationDate
                            customerId
                            socialName
                            status
                            totalNet
                            discount
                            documentTypeId
                            lines {
                                id
                                order
                                productId
                                productCode
                                productName
                                totalQuantity
                                unitPrice
                                firstDiscount
                                totalNet
                            }
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<SalesOrdersResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.SalesOrders;
        }

        /// <summary>
        /// Creates a sales order.
        /// </summary>
        public async Task<string> CreateOrderAsync(SalesOrderCreateInput input, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($values: SalesOrderCreateGLDtoInput!) {
                createSalesOrder(input: $values) {
                    id
                }
            }";

            var variables = new { values = input };
            var result = await _transport.SendQueryAsync<CreateOrderResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.CreateSalesOrder.Id;
        }

        /// <summary>
        /// Generates a compensating credit note from an existing posted sales invoice.
        /// </summary>
        public async Task<string> GenerateCreditNoteAsync(string salesInvoiceId, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($input: GenerateCreditNoteGLDtoInput!) {
                generateCreditNote(input: $input) {
                    id
                }
            }";

            var variables = new { input = new { salesInvoiceId } };
            var result = await _transport.SendQueryAsync<GenerateCreditNoteResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.GenerateCreditNote.Id;
        }

        private class SalesInvoicesResponse
        {
            [JsonPropertyName("salesInvoices")]
            public Connection<SalesInvoice> SalesInvoices { get; set; } = new Connection<SalesInvoice>();
        }

        private class CreateInvoiceResponse
        {
            [JsonPropertyName("createSalesInvoice")]
            public SalesInvoice CreateSalesInvoice { get; set; } = new SalesInvoice();
        }

        private class CloseInvoiceResponse
        {
            [JsonPropertyName("closeSalesInvoice")]
            public CloseSalesInvoiceResult CloseSalesInvoice { get; set; } = new CloseSalesInvoiceResult();
        }

        private class PostInvoiceResponse
        {
            [JsonPropertyName("postSalesInvoice")]
            public PostSalesInvoiceResult PostSalesInvoice { get; set; } = new PostSalesInvoiceResult();
        }

        private class OpenItemsResponse
        {
            [JsonPropertyName("salesInvoiceOpenItems")]
            public Connection<SalesInvoiceOpenItem> SalesInvoiceOpenItems { get; set; } = new Connection<SalesInvoiceOpenItem>();
        }

        private class SettlementResponse
        {
            [JsonPropertyName("salesOpenItemSettlement")]
            public SalesOpenItemSettlementResult SalesOpenItemSettlement { get; set; } = new SalesOpenItemSettlementResult();
        }

        private class SalesQuotesResponse
        {
            [JsonPropertyName("salesQuotes")]
            public Connection<SalesQuote> SalesQuotes { get; set; } = new Connection<SalesQuote>();
        }

        private class CreateQuoteResponse
        {
            [JsonPropertyName("createSalesQuote")]
            public SalesQuoteCreatedResult CreateSalesQuote { get; set; } = new SalesQuoteCreatedResult();
        }

        private class SalesOrdersResponse
        {
            [JsonPropertyName("salesOrders")]
            public Connection<SalesOrder> SalesOrders { get; set; } = new Connection<SalesOrder>();
        }

        private class CreateOrderResponse
        {
            [JsonPropertyName("createSalesOrder")]
            public SalesOrderCreatedResult CreateSalesOrder { get; set; } = new SalesOrderCreatedResult();
        }

        private class GenerateCreditNoteResponse
        {
            [JsonPropertyName("generateCreditNote")]
            public CreditNoteGeneratedResult GenerateCreditNote { get; set; } = new CreditNoteGeneratedResult();
        }
    }
}
