using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;
using Sage.Active.Models.GraphQL;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class PurchasesClient
    {
        private readonly SageGraphQLClient _transport;

        public PurchasesClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves purchase invoices.
        /// </summary>
        public async Task<Connection<PurchaseInvoice>> GetInvoicesAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                purchaseInvoices(order: [{ documentDate: DESC }], first: $first) {
                    edges {
                        node {
                            id
                            operationalNumber
                            externalInvoiceNumber
                            documentDate
                            status
                            totalNet
                            totalLiquid
                            supplier {
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
            var result = await _transport.SendQueryAsync<PurchaseInvoicesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.PurchaseInvoices;
        }

        /// <summary>
        /// Posts a purchase invoice to the General Ledger.
        /// </summary>
        public async Task<PostPurchaseInvoiceResult> PostInvoiceAsync(string invoiceId, string? journalTypeId = null, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($input: PostPurchaseInvoiceGLDtoInput!) {
                postPurchaseInvoice(input: $input) {
                    accountingEntryId
                    accountingEntryNumber
                }
            }";

            var variables = new { input = new { id = invoiceId, journalTypeId } };
            var result = await _transport.SendQueryAsync<PostInvoiceResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.PostPurchaseInvoice;
        }

        /// <summary>
        /// Records a payment settlement for an open payables item.
        /// </summary>
        public async Task<PurchaseOpenItemSettlementResult> SettleOpenItemAsync(PurchaseOpenItemSettlementInput input, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($input: PurchaseOpenItemSettlementGLDtoInput!) {
                purchaseOpenItemSettlement(input: $input) {
                    accountingEntryId
                    accountingEntryNumber
                }
            }";

            var variables = new { input };
            var result = await _transport.SendQueryAsync<SettlementResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.PurchaseOpenItemSettlement;
        }

        /// <summary>
        /// Ingests a receipt, bill, or invoice image/PDF for automated OCR extraction into AP automation.
        /// </summary>
        public async Task<string> UploadReceiptAsync(Stream fileStream, string fileName, string? comment = null, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($input: UploadFileInput!) {
                uploadFileToEntity(input: $input) {
                    id
                }
            }";

            var variables = new
            {
                input = new
                {
                    file = (object?)null,
                    entityType = "AP_AUTOMATION",
                    entityId = Guid.Empty.ToString(),
                    businessDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"),
                    comment = comment ?? ""
                }
            };

            var result = await _transport.SendMultipartAsync<UploadFileResponse>(mutation, variables, fileStream, fileName, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.UploadFileToEntity.Id;
        }

        /// <summary>
        /// Retrieves purchase invoices associated with an uploaded file/receipt ID.
        /// </summary>
        public async Task<Connection<PurchaseInvoice>> GetInvoicesByFileIdAsync(string fileId, int first = 20, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($fileId: UUID!, $first: Int!) {
                purchaseInvoices(where: { uploadFileId: { eq: $fileId } }, first: $first) {
                    edges {
                        node {
                            id
                            operationalNumber
                            externalInvoiceNumber
                            documentDate
                            status
                            totalNet
                            totalLiquid
                            uploadFileId
                            supplier {
                                id
                                code
                                socialName
                            }
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { fileId, first };
            var result = await _transport.SendQueryAsync<PurchaseInvoicesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.PurchaseInvoices;
        }

        /// <summary>
        /// Retrieves open items (due payment installments) for purchase invoices.
        /// </summary>
        public async Task<Connection<PurchaseInvoiceOpenItem>> GetOpenItemsAsync(string? purchaseInvoiceId = null, int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                purchaseInvoiceOpenItems(order: [{ dueDate: ASC }], first: $first) {
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

            var variables = new { first };
            var result = await _transport.SendQueryAsync<OpenItemsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.PurchaseInvoiceOpenItems;
        }

        private class PurchaseInvoicesResponse
        {
            [JsonPropertyName("purchaseInvoices")]
            public Connection<PurchaseInvoice> PurchaseInvoices { get; set; } = new Connection<PurchaseInvoice>();
        }

        private class PostInvoiceResponse
        {
            [JsonPropertyName("postPurchaseInvoice")]
            public PostPurchaseInvoiceResult PostPurchaseInvoice { get; set; } = new PostPurchaseInvoiceResult();
        }

        private class SettlementResponse
        {
            [JsonPropertyName("purchaseOpenItemSettlement")]
            public PurchaseOpenItemSettlementResult PurchaseOpenItemSettlement { get; set; } = new PurchaseOpenItemSettlementResult();
        }

        private class UploadFileResponse
        {
            [JsonPropertyName("uploadFileToEntity")]
            public UploadedFile UploadFileToEntity { get; set; } = new UploadedFile();
        }

        private class OpenItemsResponse
        {
            [JsonPropertyName("purchaseInvoiceOpenItems")]
            public Connection<PurchaseInvoiceOpenItem> PurchaseInvoiceOpenItems { get; set; } = new Connection<PurchaseInvoiceOpenItem>();
        }
    }
}
