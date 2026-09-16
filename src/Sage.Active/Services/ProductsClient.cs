using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;
using Sage.Active.Models.GraphQL;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class ProductsClient
    {
        private readonly SageGraphQLClient _transport;

        public ProductsClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves products from the product catalog.
        /// </summary>
        public async Task<Connection<Product>> GetProductsAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                products(
                    order: [{ code: ASC }]
                    where: { disabled: { eq: false } }
                    first: $first
                ) {
                    edges {
                        node {
                            id
                            code
                            name
                            description
                            category
                            disabled
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<ProductsResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Products;
        }

        /// <summary>
        /// Retrieves calculated dynamic pricing for a product.
        /// </summary>
        public async Task<ProductPrice> GetPriceByIdAsync(string productId, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($productId: UUID!) {
                productPriceById(productId: $productId) {
                    productId
                    price
                    currencyCode
                }
            }";

            var variables = new { productId };
            var result = await _transport.SendQueryAsync<PriceResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.ProductPriceById;
        }

        private class ProductsResponse
        {
            [JsonPropertyName("products")]
            public Connection<Product> Products { get; set; } = new Connection<Product>();
        }

        private class PriceResponse
        {
            [JsonPropertyName("productPriceById")]
            public ProductPrice ProductPriceById { get; set; } = new ProductPrice();
        }
    }
}
