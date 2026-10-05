using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Auth;
using Sage.Active.Models;
using Sage.Active.Models.GraphQL;
using Sage.Active.Services;
using Sage.Active.Transport;

namespace Sage.Active
{
    /// <summary>
    /// Master client for the Sage Active Public API V2 suite.
    /// Provides strongly-typed access to all accounting, sales, purchases, banking, master data, and analytics services.
    /// </summary>
    public class SageActiveClient
    {
        private readonly SageGraphQLClient _transport;

        public SageActiveConfig Config => _transport.Config;
        public SageAuthClient Auth => _transport.Auth;
        public SageGraphQLClient Transport => _transport;

        public OrganizationsClient Organizations { get; }
        public UsersClient Users { get; }
        public AccountingClient Accounting { get; }
        public ThirdPartiesClient ThirdParties { get; }
        public ProductsClient Products { get; }
        public SalesClient Sales { get; }
        public PurchasesClient Purchases { get; }
        public BanksClient Banks { get; }
        public FilesClient Files { get; }
        public CatalogClient Catalog { get; }
        public LocalizationClient Localization { get; }

        public SageActiveClient(SageActiveConfig config, HttpClient? httpClient = null)
            : this(new SageGraphQLClient(config, httpClient))
        {
        }

        internal SageActiveClient(SageGraphQLClient transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));

            Organizations = new OrganizationsClient(_transport);
            Users = new UsersClient(_transport);
            Accounting = new AccountingClient(_transport);
            ThirdParties = new ThirdPartiesClient(_transport);
            Products = new ProductsClient(_transport);
            Sales = new SalesClient(_transport);
            Purchases = new PurchasesClient(_transport);
            Banks = new BanksClient(_transport);
            Files = new FilesClient(_transport);
            Catalog = new CatalogClient(_transport);
            Localization = new LocalizationClient(_transport);
        }

        /// <summary>
        /// Creates a scoped clone of this client configured for a specific organization/tenant ID.
        /// Reuses the underlying HTTP connection pool, token cache, and mutation concurrency throttler.
        /// </summary>
        public SageActiveClient WithOrganization(string organizationId)
        {
            if (string.IsNullOrWhiteSpace(organizationId))
                throw new ArgumentException("Organization ID cannot be null or whitespace.", nameof(organizationId));

            var clonedConfig = new SageActiveConfig
            {
                Region = Config.Region,
                Environment = Config.Environment,
                BaseAddress = Config.BaseAddress,
                AuthUrl = Config.AuthUrl,
                AccessTokenUrl = Config.AccessTokenUrl,
                ClientId = Config.ClientId,
                ClientSecret = Config.ClientSecret,
                SubscriptionKey = Config.SubscriptionKey,
                OrganizationId = organizationId,
                AccessToken = Config.AccessToken,
                RefreshToken = Config.RefreshToken,
                Scopes = Config.Scopes,
                CallbackUrl = Config.CallbackUrl,
                Timeout = Config.Timeout,
                MaxRetryAttempts = Config.MaxRetryAttempts,
                MaxConcurrentMutations = Config.MaxConcurrentMutations
            };

            var transport = new SageGraphQLClient(clonedConfig, _transport.HttpClient, _transport.Auth, _transport.MutationSemaphore);
            return new SageActiveClient(transport);
        }

        /// <summary>
        /// Direct escape hatch: Executes any custom GraphQL query or mutation.
        /// </summary>
        public Task<TData> ExecuteQueryAsync<TData>(string query, object? variables = null, string? operationName = null, CancellationToken cancellationToken = default)
        {
            return _transport.SendQueryAsync<TData>(query, variables, operationName, cancellationToken);
        }

        /// <summary>
        /// Direct escape hatch: Sends a raw GraphQLRequest payload.
        /// </summary>
        public Task<GraphQLResponse<TData>> ExecuteRawAsync<TData>(GraphQLRequest request, CancellationToken cancellationToken = default)
        {
            return _transport.SendRequestAsync<TData>(request, cancellationToken);
        }

        /// <summary>
        /// Quick factory method to create an instance configured for a specific region.
        /// </summary>
        public static SageActiveClient Create(string subscriptionKey, string organizationId, SageRegion region = SageRegion.FR, SageEnvironment environment = SageEnvironment.Production)
        {
            return new SageActiveClient(new SageActiveConfig
            {
                SubscriptionKey = subscriptionKey,
                OrganizationId = organizationId,
                Region = region,
                Environment = environment
            });
        }
    }
}
