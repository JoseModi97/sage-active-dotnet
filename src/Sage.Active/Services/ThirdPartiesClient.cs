using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;
using Sage.Active.Models.GraphQL;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class ThirdPartiesClient
    {
        private readonly SageGraphQLClient _transport;

        public ThirdPartiesClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves customers with optional active filter.
        /// </summary>
        public async Task<Connection<Customer>> GetCustomersAsync(bool onlyActive = true, int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!, $disabled: Boolean!) {
                customers(
                    order: [{ code: ASC }]
                    where: { disabled: { eq: $disabled } }
                    first: $first
                ) {
                    edges {
                        node {
                            id
                            code
                            socialName
                            tradeName
                            vatNumber
                            documentId
                            disabled
                            countryAcronym
                            addresses {
                                id
                                firstLine
                                city
                                zipCode
                                countryIsoCodeAlpha2
                                isDefault
                            }
                            contacts {
                                id
                                name
                                surname
                                courtesy
                                isDefault
                                phones {
                                    number
                                    type
                                    isDefault
                                }
                                emails {
                                    emailAddress
                                    usage
                                    isDefault
                                }
                            }
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first, disabled = !onlyActive };
            var result = await _transport.SendQueryAsync<CustomersResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Customers;
        }

        /// <summary>
        /// Creates a new customer record.
        /// </summary>
        public async Task<Customer> CreateCustomerAsync(CustomerCreateInput input, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($values: CustomerCreateGLDtoInput!) {
                createCustomer(input: $values) {
                    id
                    code
                    socialName
                }
            }";

            var variables = new { values = input };
            var result = await _transport.SendQueryAsync<CreateCustomerResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.CreateCustomer;
        }

        /// <summary>
        /// Deletes a customer by ID.
        /// </summary>
        public async Task<bool> DeleteCustomerAsync(string customerId, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($id: UUID!) {
                deleteCustomer(id: $id)
            }";

            var variables = new { id = customerId };
            var result = await _transport.SendQueryAsync<DeleteCustomerResponse>(mutation, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Deleted;
        }

        /// <summary>
        /// Retrieves suppliers.
        /// </summary>
        public async Task<Connection<Supplier>> GetSuppliersAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                suppliers(order: [{ code: ASC }], first: $first) {
                    edges {
                        node {
                            id
                            code
                            socialName
                            tradeName
                            vatNumber
                            disabled
                            taxTreatmentId
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<SuppliersResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Suppliers;
        }

        private class CustomersResponse
        {
            [JsonPropertyName("customers")]
            public Connection<Customer> Customers { get; set; } = new Connection<Customer>();
        }

        private class CreateCustomerResponse
        {
            [JsonPropertyName("createCustomer")]
            public Customer CreateCustomer { get; set; } = new Customer();
        }

        private class DeleteCustomerResponse
        {
            [JsonPropertyName("deleteCustomer")]
            public bool Deleted { get; set; }
        }

        private class SuppliersResponse
        {
            [JsonPropertyName("suppliers")]
            public Connection<Supplier> Suppliers { get; set; } = new Connection<Supplier>();
        }
    }
}
