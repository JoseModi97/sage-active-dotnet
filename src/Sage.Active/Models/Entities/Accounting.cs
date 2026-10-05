using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.Entities
{
    public class AccountingAccount
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("taxTreatmentId")]
        public string? TaxTreatmentId { get; set; }
    }

    public class AccountingExercise
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("startDate")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class JournalType
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    public class Tax
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("rate")]
        public decimal? Rate { get; set; }

        [JsonPropertyName("groupName")]
        public string? GroupName { get; set; }

        [JsonPropertyName("groupId")]
        public string? GroupId { get; set; }

        [JsonPropertyName("percentage")]
        public decimal? Percentage { get; set; }

        [JsonPropertyName("equivalenceSurchargePercentage")]
        public decimal? EquivalenceSurchargePercentage { get; set; }

        [JsonPropertyName("hasEquivalenceSurcharge")]
        public bool? HasEquivalenceSurcharge { get; set; }

        [JsonPropertyName("taxType")]
        public string? TaxType { get; set; }

        [JsonPropertyName("inactive")]
        public bool? Inactive { get; set; }

        [JsonPropertyName("effectiveDate")]
        public DateTimeOffset? EffectiveDate { get; set; }

        [JsonPropertyName("inactivationDate")]
        public DateTimeOffset? InactivationDate { get; set; }
    }

    public class TaxGroup
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("taxGroupCode")]
        public string? TaxGroupCode { get; set; }

        [JsonPropertyName("taxType")]
        public string? TaxType { get; set; }

        [JsonPropertyName("vatTaxation")]
        public string? VatTaxation { get; set; }
    }

    public class TaxTreatment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class PaymentTerm
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("modificationDate")]
        public DateTimeOffset? ModificationDate { get; set; }

        [JsonPropertyName("lines")]
        public List<PaymentTermLine>? Lines { get; set; }
    }

    public class PaymentTermLine
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("paymentMeanId")]
        public string? PaymentMeanId { get; set; }

        [JsonPropertyName("day")]
        public int? Day { get; set; }

        [JsonPropertyName("condition")]
        public string? Condition { get; set; }

        [JsonPropertyName("order")]
        public int? Order { get; set; }

        [JsonPropertyName("payDays")]
        public string? PayDays { get; set; }
    }

    public class AccountingEntry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [JsonPropertyName("date")]
        public DateTimeOffset Date { get; set; }

        [JsonPropertyName("documentDate")]
        public DateTimeOffset? DocumentDate { get; set; }

        [JsonPropertyName("documentNumber")]
        public string? DocumentNumber { get; set; }

        [JsonPropertyName("journalTypeId")]
        public string? JournalTypeId { get; set; }

        [JsonPropertyName("journalType")]
        public JournalType? JournalType { get; set; }

        [JsonPropertyName("accountingEntryLines")]
        public List<AccountingEntryLine>? Lines { get; set; }
    }

    public class AccountingEntryLine
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("debitAmount")]
        public decimal DebitAmount { get; set; }

        [JsonPropertyName("creditAmount")]
        public decimal CreditAmount { get; set; }

        [JsonPropertyName("subAccountId")]
        public string? SubAccountId { get; set; }

        [JsonPropertyName("subAccountCode")]
        public string? SubAccountCode { get; set; }

        [JsonPropertyName("thirdCode")]
        public string? ThirdCode { get; set; }
    }

    public class AccountingEntryCreateUsingCodesInput
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("documentDate")]
        public string? DocumentDate { get; set; }

        [JsonPropertyName("documentNumber")]
        public string? DocumentNumber { get; set; }

        [JsonPropertyName("journalTypeCode")]
        public string JournalTypeCode { get; set; } = string.Empty;

        [JsonPropertyName("accountingEntryLines")]
        public List<AccountingEntryLineInput> Lines { get; set; } = new List<AccountingEntryLineInput>();
    }

    public class AccountingEntryLineInput
    {
        [JsonPropertyName("subAccountCode")]
        public string SubAccountCode { get; set; } = string.Empty;

        [JsonPropertyName("thirdCode")]
        public string? ThirdCode { get; set; }

        [JsonPropertyName("debitAmount")]
        public decimal DebitAmount { get; set; }

        [JsonPropertyName("creditAmount")]
        public decimal CreditAmount { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class AccountingEntryCreatedResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("number")]
        public string? Number { get; set; }
    }
}
