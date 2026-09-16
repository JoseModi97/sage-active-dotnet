using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.Entities
{
    public class SalesInvoice
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("operationalNumber")]
        public string? OperationalNumber { get; set; }

        [JsonPropertyName("documentDate")]
        public DateTimeOffset DocumentDate { get; set; }

        [JsonPropertyName("firstDueDate")]
        public DateTimeOffset? FirstDueDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("totalNet")]
        public decimal TotalNet { get; set; }

        [JsonPropertyName("totalLiquid")]
        public decimal TotalLiquid { get; set; }

        [JsonPropertyName("customer")]
        public Customer? Customer { get; set; }

        [JsonPropertyName("customerId")]
        public string? CustomerId { get; set; }

        [JsonPropertyName("salesInvoiceLines")]
        public List<SalesInvoiceLine>? Lines { get; set; }
    }

    public class SalesInvoiceLine
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("product")]
        public Product? Product { get; set; }

        [JsonPropertyName("productId")]
        public string? ProductId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }

    public class SalesInvoiceOpenItem
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

        [JsonPropertyName("salesInvoice")]
        public SalesInvoice? SalesInvoice { get; set; }
    }

    public class SalesInvoiceCreateInput
    {
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; } = string.Empty;

        [JsonPropertyName("documentDate")]
        public string DocumentDate { get; set; } = string.Empty;

        [JsonPropertyName("salesInvoiceLines")]
        public List<SalesInvoiceLineInput> Lines { get; set; } = new List<SalesInvoiceLineInput>();
    }

    public class SalesInvoiceLineInput
    {
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; } = 1;

        [JsonPropertyName("unitPrice")]
        public decimal? UnitPrice { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class CloseSalesInvoiceResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("operationalNumber")]
        public string OperationalNumber { get; set; } = string.Empty;
    }

    public class PostSalesInvoiceResult
    {
        [JsonPropertyName("accountingEntryId")]
        public string? AccountingEntryId { get; set; }

        [JsonPropertyName("accountingEntryNumber")]
        public string? AccountingEntryNumber { get; set; }
    }

    public class SalesOpenItemSettlementInput
    {
        [JsonPropertyName("salesInvoiceOpenItemId")]
        public string SalesInvoiceOpenItemId { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("paymentMethodId")]
        public string PaymentMethodId { get; set; } = string.Empty;

        [JsonPropertyName("settlementDate")]
        public string SettlementDate { get; set; } = string.Empty;
    }

    public class SalesOpenItemSettlementResult
    {
        [JsonPropertyName("accountingEntryId")]
        public string? AccountingEntryId { get; set; }

        [JsonPropertyName("accountingEntryNumber")]
        public string? AccountingEntryNumber { get; set; }
    }
}
