using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.Entities
{
    public class Customer
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("socialName")]
        public string SocialName { get; set; } = string.Empty;

        [JsonPropertyName("tradeName")]
        public string? TradeName { get; set; }

        [JsonPropertyName("vatNumber")]
        public string? VatNumber { get; set; }

        [JsonPropertyName("documentId")]
        public string? DocumentId { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }

        [JsonPropertyName("countryAcronym")]
        public string? CountryAcronym { get; set; }

        [JsonPropertyName("addresses")]
        public List<Address>? Addresses { get; set; }

        [JsonPropertyName("contacts")]
        public List<Contact>? Contacts { get; set; }
    }

    public class Supplier
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("socialName")]
        public string SocialName { get; set; } = string.Empty;

        [JsonPropertyName("tradeName")]
        public string? TradeName { get; set; }

        [JsonPropertyName("vatNumber")]
        public string? VatNumber { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }

        [JsonPropertyName("taxTreatmentId")]
        public string? TaxTreatmentId { get; set; }
    }

    public class Employee
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [JsonPropertyName("vatNumber")]
        public string? VatNumber { get; set; }
    }

    public class Address
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("firstLine")]
        public string? FirstLine { get; set; }

        [JsonPropertyName("secondLine")]
        public string? SecondLine { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("countryIsoCodeAlpha2")]
        public string? CountryIsoCodeAlpha2 { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }

    public class Contact
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [JsonPropertyName("courtesy")]
        public string? Courtesy { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("phones")]
        public List<Phone>? Phones { get; set; }

        [JsonPropertyName("emails")]
        public List<Email>? Emails { get; set; }
    }

    public class Phone
    {
        [JsonPropertyName("number")]
        public string Number { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }

    public class Email
    {
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = string.Empty;

        [JsonPropertyName("usage")]
        public string? Usage { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }

    public class CustomerCreateInput
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("socialName")]
        public string SocialName { get; set; } = string.Empty;

        [JsonPropertyName("tradeName")]
        public string? TradeName { get; set; }

        [JsonPropertyName("vatNumber")]
        public string? VatNumber { get; set; }

        [JsonPropertyName("documentId")]
        public string? DocumentId { get; set; }

        [JsonPropertyName("addresses")]
        public List<Address>? Addresses { get; set; }

        [JsonPropertyName("contacts")]
        public List<Contact>? Contacts { get; set; }
    }

    public class CustomerUpdateInput : CustomerCreateInput
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
