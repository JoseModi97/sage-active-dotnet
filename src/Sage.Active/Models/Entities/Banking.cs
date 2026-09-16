using System;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.Entities
{
    public class BankAccount
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("iban")]
        public string? Iban { get; set; }

        [JsonPropertyName("bic")]
        public string? Bic { get; set; }

        [JsonPropertyName("bankName")]
        public string? BankName { get; set; }

        [JsonPropertyName("connectedBankId")]
        public string? ConnectedBankId { get; set; }
    }

    public class BankMovement
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public DateTimeOffset Date { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("isReconciled")]
        public bool IsReconciled { get; set; }
    }

    public class PaymentMethod
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("referenceName")]
        public string ReferenceName { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
