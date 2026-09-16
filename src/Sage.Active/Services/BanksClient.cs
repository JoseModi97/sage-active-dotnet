using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;
using Sage.Active.Models.GraphQL;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class BanksClient
    {
        private readonly SageGraphQLClient _transport;

        public BanksClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves connected bank accounts.
        /// </summary>
        public async Task<Connection<BankAccount>> GetBankAccountsAsync(int first = 20, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                bankAccounts(first: $first) {
                    edges {
                        node {
                            id
                            iban
                            bic
                            bankName
                            connectedBankId
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<BankAccountsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.BankAccounts;
        }

        /// <summary>
        /// Retrieves bank movements (statement transactions) with optional bank account filter.
        /// </summary>
        public async Task<Connection<BankMovement>> GetBankMovementsAsync(string? connectedBankId = null, int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                bankMovements(order: [{ date: DESC }], first: $first) {
                    edges {
                        node {
                            id
                            date
                            amount
                            description
                            isReconciled
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<BankMovementsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.BankMovements;
        }

        /// <summary>
        /// Reconciles a bank movement with an accounting payment entry.
        /// </summary>
        public async Task<bool> ReconcileMovementAsync(string bankMovementId, string accountingEntryId, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($input: ReconcileBankMovementInput!) {
                reconcileBankMovement(input: $input)
            }";

            var variables = new { input = new { bankMovementId, accountingEntryId } };
            var result = await _transport.SendQueryAsync<ReconcileResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Success;
        }

        /// <summary>
        /// Retrieves configured payment methods.
        /// </summary>
        public async Task<Connection<PaymentMethod>> GetPaymentMethodsAsync(int first = 20, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                paymentMethods(first: $first) {
                    edges {
                        node {
                            id
                            referenceName
                            type
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<PaymentMethodsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.PaymentMethods;
        }

        private class BankAccountsResponse
        {
            [JsonPropertyName("bankAccounts")]
            public Connection<BankAccount> BankAccounts { get; set; } = new Connection<BankAccount>();
        }

        private class BankMovementsResponse
        {
            [JsonPropertyName("bankMovements")]
            public Connection<BankMovement> BankMovements { get; set; } = new Connection<BankMovement>();
        }

        private class ReconcileResponse
        {
            [JsonPropertyName("reconcileBankMovement")]
            public bool Success { get; set; }
        }

        private class PaymentMethodsResponse
        {
            [JsonPropertyName("paymentMethods")]
            public Connection<PaymentMethod> PaymentMethods { get; set; } = new Connection<PaymentMethod>();
        }
    }
}
