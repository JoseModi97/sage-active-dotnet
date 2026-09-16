using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;
using Sage.Active.Models.GraphQL;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class OrganizationsClient
    {
        private readonly SageGraphQLClient _transport;

        public OrganizationsClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves all organizations available to the authenticated account.
        /// </summary>
        public async Task<Connection<Organization>> GetOrganizationsAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                organizations(
                    where: { status: { eq: READY }, onboardingCompleted: { eq: true } }
                    order: { creationDate: DESC }
                    first: $first
                ) {
                    edges {
                        node {
                            id
                            socialName
                            legislationCode
                            status
                            onboardingCompleted
                            creationDate
                        }
                    }
                    totalCount
                    pageInfo {
                        hasNextPage
                        endCursor
                    }
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<OrganizationsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Organizations;
        }

        /// <summary>
        /// Queries available countries for internationalization and address creation.
        /// </summary>
        public async Task<Connection<Country>> GetCountriesAsync(int first = 100, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                countries(first: $first, order: [{ name: ASC }]) {
                    edges {
                        node {
                            isoCodeAlpha2
                            isoCodeAlpha3
                            name
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<CountriesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Countries;
        }

        /// <summary>
        /// Queries available currencies.
        /// </summary>
        public async Task<Connection<Currency>> GetCurrenciesAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                currencies(first: $first) {
                    edges {
                        node {
                            code
                            name
                            symbol
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<CurrenciesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Currencies;
        }

        private class OrganizationsResponse
        {
            [JsonPropertyName("organizations")]
            public Connection<Organization> Organizations { get; set; } = new Connection<Organization>();
        }

        private class CountriesResponse
        {
            [JsonPropertyName("countries")]
            public Connection<Country> Countries { get; set; } = new Connection<Country>();
        }

        private class CurrenciesResponse
        {
            [JsonPropertyName("currencies")]
            public Connection<Currency> Currencies { get; set; } = new Connection<Currency>();
        }
    }
}
