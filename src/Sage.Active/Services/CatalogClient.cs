using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class CatalogClient
    {
        private readonly SageGraphQLClient _transport;

        public CatalogClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves the available analytical aggregation definitions.
        /// </summary>
        public async Task<List<AggregationCatalogDefinition>> GetAggregationCatalogAsync(string? contextContains = null, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($contextContains: String) {
                aggregationCatalog(contextContains: $contextContains) {
                    definitions {
                        entityKey
                        context
                        aggregationTypes
                        periodTypes
                        filters
                        valueColumns
                    }
                }
            }";

            var variables = new { contextContains };
            var result = await _transport.SendQueryAsync<AggregationCatalogResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.AggregationCatalog.Definitions;
        }

        /// <summary>
        /// Executes a multidimensional aggregation cube (e.g. Sales by Customer or Month).
        /// </summary>
        public async Task<AggregationExecutionResult> ExecuteAggregationAsync(AggregationExecuteInput input, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($input: AggregationExecuteDtoInput!) {
                aggregationExecute(input: $input) {
                    rows {
                        groupValue
                        period
                        value1
                        value2
                        deltaPercent
                    }
                }
            }";

            var variables = new { input };
            var result = await _transport.SendQueryAsync<AggregationExecuteResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.AggregationExecute;
        }

        private class AggregationCatalogResponse
        {
            [JsonPropertyName("aggregationCatalog")]
            public AggregationCatalogWrapper AggregationCatalog { get; set; } = new AggregationCatalogWrapper();
        }

        private class AggregationCatalogWrapper
        {
            [JsonPropertyName("definitions")]
            public List<AggregationCatalogDefinition> Definitions { get; set; } = new List<AggregationCatalogDefinition>();
        }

        private class AggregationExecuteResponse
        {
            [JsonPropertyName("aggregationExecute")]
            public AggregationExecutionResult AggregationExecute { get; set; } = new AggregationExecutionResult();
        }
    }
}
