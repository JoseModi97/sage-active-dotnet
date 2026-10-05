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

    public class SalesQuote
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("operationalNumber")]
        public string? OperationalNumber { get; set; }

        [JsonPropertyName("documentDate")]
        public DateTimeOffset DocumentDate { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTimeOffset? CreationDate { get; set; }

        [JsonPropertyName("customerId")]
        public string? CustomerId { get; set; }

        [JsonPropertyName("socialName")]
        public string? SocialName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("declinedReason")]
        public string? DeclinedReason { get; set; }

        [JsonPropertyName("totalNet")]
        public decimal TotalNet { get; set; }

        [JsonPropertyName("discount")]
        public decimal? Discount { get; set; }

        [JsonPropertyName("documentTypeId")]
        public string? DocumentTypeId { get; set; }

        [JsonPropertyName("lines")]
        public List<SalesQuoteLine>? Lines { get; set; }
    }

    public class SalesQuoteLine
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("order")]
        public int? Order { get; set; }

        [JsonPropertyName("productId")]
        public string? ProductId { get; set; }

        [JsonPropertyName("productCode")]
        public string? ProductCode { get; set; }

        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("totalQuantity")]
        public decimal TotalQuantity { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("firstDiscount")]
        public decimal? FirstDiscount { get; set; }

        [JsonPropertyName("totalNet")]
        public decimal TotalNet { get; set; }
    }

    public class SalesQuoteCreateInput
    {
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; } = string.Empty;

        [JsonPropertyName("documentDate")]
        public string DocumentDate { get; set; } = string.Empty;

        [JsonPropertyName("salesQuoteLines")]
        public List<SalesQuoteLineInput> Lines { get; set; } = new List<SalesQuoteLineInput>();
    }

    public class SalesQuoteLineInput
    {
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;

        [JsonPropertyName("totalQuantity")]
        public decimal TotalQuantity { get; set; } = 1;

        [JsonPropertyName("unitPrice")]
        public decimal? UnitPrice { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class SalesQuoteCreatedResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("operationalNumber")]
        public string? OperationalNumber { get; set; }
    }

    public class SalesOrder
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("operationalNumber")]
        public string? OperationalNumber { get; set; }

        [JsonPropertyName("documentDate")]
        public DateTimeOffset DocumentDate { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTimeOffset? CreationDate { get; set; }

        [JsonPropertyName("customerId")]
        public string? CustomerId { get; set; }

        [JsonPropertyName("socialName")]
        public string? SocialName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("totalNet")]
        public decimal TotalNet { get; set; }

        [JsonPropertyName("discount")]
        public decimal? Discount { get; set; }

        [JsonPropertyName("documentTypeId")]
        public string? DocumentTypeId { get; set; }

        [JsonPropertyName("lines")]
        public List<SalesOrderLine>? Lines { get; set; }
    }

    public class SalesOrderLine
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("order")]
        public int? Order { get; set; }

        [JsonPropertyName("productId")]
        public string? ProductId { get; set; }

        [JsonPropertyName("productCode")]
        public string? ProductCode { get; set; }

        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("totalQuantity")]
        public decimal TotalQuantity { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("firstDiscount")]
        public decimal? FirstDiscount { get; set; }

        [JsonPropertyName("totalNet")]
        public decimal TotalNet { get; set; }
    }

    public class SalesOrderCreateInput
    {
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; } = string.Empty;

        [JsonPropertyName("documentDate")]
        public string DocumentDate { get; set; } = string.Empty;

        [JsonPropertyName("salesOrderLines")]
        public List<SalesOrderLineInput> Lines { get; set; } = new List<SalesOrderLineInput>();
    }

    public class SalesOrderLineInput
    {
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;

        [JsonPropertyName("totalQuantity")]
        public decimal TotalQuantity { get; set; } = 1;

        [JsonPropertyName("unitPrice")]
        public decimal? UnitPrice { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class SalesOrderCreatedResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("operationalNumber")]
        public string? OperationalNumber { get; set; }
    }

    public class CreditNoteGeneratedResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
