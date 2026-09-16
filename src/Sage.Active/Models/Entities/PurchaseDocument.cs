using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.Entities
{
    public class PurchaseInvoice
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("operationalNumber")]
        public string? OperationalNumber { get; set; }

        [JsonPropertyName("externalInvoiceNumber")]
        public string? ExternalInvoiceNumber { get; set; }

        [JsonPropertyName("documentDate")]
        public DateTimeOffset DocumentDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("totalNet")]
        public decimal TotalNet { get; set; }

        [JsonPropertyName("totalLiquid")]
        public decimal TotalLiquid { get; set; }

        [JsonPropertyName("supplier")]
        public Supplier? Supplier { get; set; }

        [JsonPropertyName("supplierId")]
        public string? SupplierId { get; set; }

        [JsonPropertyName("uploadFileId")]
        public string? UploadFileId { get; set; }
    }

    public class PurchaseInvoiceOpenItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("dueDate")]
        public DateTimeOffset DueDate { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("paidAmountAccumulated")]
        public decimal PaidAmountAccumulated { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class PurchaseInvoiceCreateInput
    {
        [JsonPropertyName("supplierId")]
        public string SupplierId { get; set; } = string.Empty;

        [JsonPropertyName("documentDate")]
        public string DocumentDate { get; set; } = string.Empty;

        [JsonPropertyName("externalInvoiceNumber")]
        public string? ExternalInvoiceNumber { get; set; }

        [JsonPropertyName("totalNet")]
        public decimal TotalNet { get; set; }

        [JsonPropertyName("totalTax")]
        public decimal TotalTax { get; set; }

        [JsonPropertyName("totalLiquid")]
        public decimal TotalLiquid { get; set; }
    }

    public class PostPurchaseInvoiceResult
    {
        [JsonPropertyName("accountingEntryId")]
        public string? AccountingEntryId { get; set; }

        [JsonPropertyName("accountingEntryNumber")]
        public string? AccountingEntryNumber { get; set; }
    }

    public class PurchaseOpenItemSettlementInput
    {
        [JsonPropertyName("purchaseInvoiceOpenItemId")]
        public string PurchaseInvoiceOpenItemId { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("paymentMethodId")]
        public string PaymentMethodId { get; set; } = string.Empty;

        [JsonPropertyName("settlementDate")]
        public string SettlementDate { get; set; } = string.Empty;
    }

    public class PurchaseOpenItemSettlementResult
    {
        [JsonPropertyName("accountingEntryId")]
        public string? AccountingEntryId { get; set; }

        [JsonPropertyName("accountingEntryNumber")]
        public string? AccountingEntryNumber { get; set; }
    }
}
