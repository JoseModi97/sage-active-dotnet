using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;
using Sage.Active.Models.GraphQL;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class AccountingClient
    {
        private readonly SageGraphQLClient _transport;

        public AccountingClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves open or historical accounting exercises (fiscal years).
        /// </summary>
        public async Task<Connection<AccountingExercise>> GetExercisesAsync(int first = 10, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                accountingExercises(order: { startDate: DESC }, first: $first) {
                    edges {
                        node {
                            id
                            code
                            startDate
                            endDate
                            status
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<ExercisesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Exercises;
        }

        /// <summary>
        /// Retrieves chart of accounts.
        /// </summary>
        public async Task<Connection<AccountingAccount>> GetAccountsAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                accountingAccounts(order: [{ code: ASC }], first: $first) {
                    edges {
                        node {
                            id
                            code
                            name
                            description
                            taxTreatmentId
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<AccountsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Accounts;
        }

        /// <summary>
        /// Retrieves journal types (Sales, Purchases, Bank, General Ledger).
        /// </summary>
        public async Task<Connection<JournalType>> GetJournalTypesAsync(int first = 20, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                journalTypes(first: $first) {
                    edges {
                        node {
                            id
                            code
                            name
                            type
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<JournalTypesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.JournalTypes;
        }

        /// <summary>
        /// Retrieves accounting entries within a date range or journal.
        /// </summary>
        public async Task<Connection<AccountingEntry>> GetEntriesAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                accountingEntries(order: [{ date: DESC }], first: $first) {
                    edges {
                        node {
                            id
                            number
                            date
                            documentDate
                            documentNumber
                            journalType {
                                code
                                name
                            }
                            accountingEntryLines {
                                id
                                description
                                debitAmount
                                creditAmount
                                subAccountCode
                                thirdCode
                            }
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<EntriesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Entries;
        }

        /// <summary>
        /// Creates balanced accounting entry lines using human-readable account and third-party codes.
        /// </summary>
        public async Task<AccountingEntryCreatedResult> CreateEntryUsingCodesAsync(AccountingEntryCreateUsingCodesInput input, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation CreateAccountingEntryUsingCodes($values: AccountingEntryCreateUsingCodesGLDtoInput!) {
                createAccountingEntryUsingCodes(input: $values) {
                    id
                    number
                }
            }";

            var variables = new { values = input };
            var result = await _transport.SendQueryAsync<CreateEntryResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.CreateAccountingEntryUsingCodes;
        }

        /// <summary>
        /// Deletes an unposted or draft accounting entry by identifier.
        /// </summary>
        public async Task<bool> DeleteEntryAsync(string id, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($id: UUID!) {
                deleteAccountingEntry(id: $id)
            }";

            var variables = new { id };
            var result = await _transport.SendQueryAsync<DeleteEntryResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Deleted;
        }

        /// <summary>
        /// Retrieves fiscal tax rates (TVA / VAT).
        /// </summary>
        public async Task<Connection<Tax>> GetTaxesAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                taxes(first: $first) {
                    edges {
                        node {
                            id
                            code
                            name
                            rate
                            groupName
                            groupId
                            percentage
                            equivalenceSurchargePercentage
                            hasEquivalenceSurcharge
                            taxType
                            inactive
                            effectiveDate
                            inactivationDate
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<TaxesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Taxes;
        }

        /// <summary>
        /// Retrieves tax classification groups.
        /// </summary>
        public async Task<Connection<TaxGroup>> GetTaxGroupsAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                taxGroups(first: $first) {
                    edges {
                        node {
                            id
                            name
                            taxGroupCode
                            taxType
                            vatTaxation
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<TaxGroupsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.TaxGroups;
        }

        /// <summary>
        /// Retrieves tax treatments for customer and supplier fiscal categorization.
        /// </summary>
        public async Task<Connection<TaxTreatment>> GetTaxTreatmentsAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                taxTreatments(first: $first) {
                    edges {
                        node {
                            id
                            code
                            name
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<TaxTreatmentsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.TaxTreatments;
        }

        /// <summary>
        /// Retrieves commercial payment terms (conditions de paiement).
        /// </summary>
        public async Task<Connection<PaymentTerm>> GetPaymentTermsAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                paymentTerms(first: $first) {
                    edges {
                        node {
                            id
                            name
                            modificationDate
                            lines {
                                id
                                type
                                paymentMeanId
                                day
                                condition
                                order
                                payDays
                            }
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<PaymentTermsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.PaymentTerms;
        }

        private class ExercisesResponse
        {
            [JsonPropertyName("accountingExercises")]
            public Connection<AccountingExercise> Exercises { get; set; } = new Connection<AccountingExercise>();
        }

        private class AccountsResponse
        {
            [JsonPropertyName("accountingAccounts")]
            public Connection<AccountingAccount> Accounts { get; set; } = new Connection<AccountingAccount>();
        }

        private class JournalTypesResponse
        {
            [JsonPropertyName("journalTypes")]
            public Connection<JournalType> JournalTypes { get; set; } = new Connection<JournalType>();
        }

        private class EntriesResponse
        {
            [JsonPropertyName("accountingEntries")]
            public Connection<AccountingEntry> Entries { get; set; } = new Connection<AccountingEntry>();
        }

        private class CreateEntryResponse
        {
            [JsonPropertyName("createAccountingEntryUsingCodes")]
            public AccountingEntryCreatedResult CreateAccountingEntryUsingCodes { get; set; } = new AccountingEntryCreatedResult();
        }

        private class DeleteEntryResponse
        {
            [JsonPropertyName("deleteAccountingEntry")]
            public bool Deleted { get; set; }
        }

        private class TaxesResponse
        {
            [JsonPropertyName("taxes")]
            public Connection<Tax> Taxes { get; set; } = new Connection<Tax>();
        }

        private class TaxGroupsResponse
        {
            [JsonPropertyName("taxGroups")]
            public Connection<TaxGroup> TaxGroups { get; set; } = new Connection<TaxGroup>();
        }

        private class TaxTreatmentsResponse
        {
            [JsonPropertyName("taxTreatments")]
            public Connection<TaxTreatment> TaxTreatments { get; set; } = new Connection<TaxTreatment>();
        }

        private class PaymentTermsResponse
        {
            [JsonPropertyName("paymentTerms")]
            public Connection<PaymentTerm> PaymentTerms { get; set; } = new Connection<PaymentTerm>();
        }
    }
}
