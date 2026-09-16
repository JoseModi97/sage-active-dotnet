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
