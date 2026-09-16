using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sage.Active;
using Sage.Active.Models;

namespace Sage.Active.AspNetCore
{
    public static class SageActiveServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Sage Active client services with configuration action.
        /// </summary>
        public static IServiceCollection AddSageActive(this IServiceCollection services, Action<SageActiveConfig> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var config = new SageActiveConfig();
            configure(config);

            services.AddSingleton(config);
            services.AddHttpClient<SageActiveClient>();
            services.AddSingleton(sp =>
            {
                var cfg = sp.GetRequiredService<SageActiveConfig>();
                var httpFactory = sp.GetService<IHttpClientFactory>();
                var httpClient = httpFactory != null ? httpFactory.CreateClient(nameof(SageActiveClient)) : null;
                return new SageActiveClient(cfg, httpClient);
            });

            return services;
        }

        /// <summary>
        /// Registers Sage Active client services from IConfiguration (e.g. appsettings.json section "SageActive").
        /// </summary>
        public static IServiceCollection AddSageActive(this IServiceCollection services, IConfiguration configuration, string sectionName = "SageActive")
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var section = configuration.GetSection(sectionName);
            var config = new SageActiveConfig();

            // Bind configuration properties
            var regionStr = section["Region"];
            if (!string.IsNullOrWhiteSpace(regionStr) && Enum.TryParse<SageRegion>(regionStr, true, out var r))
            {
                config.Region = r;
            }

            var envStr = section["Environment"];
            if (!string.IsNullOrWhiteSpace(envStr) && Enum.TryParse<SageEnvironment>(envStr, true, out var e))
            {
                config.Environment = e;
            }

            config.ClientId = section["ClientId"] ?? config.ClientId;
            config.ClientSecret = section["ClientSecret"] ?? config.ClientSecret;
            config.SubscriptionKey = section["SubscriptionKey"] ?? section["ApiKey"] ?? config.SubscriptionKey;
            config.OrganizationId = section["OrganizationId"] ?? config.OrganizationId;
            config.AccessToken = section["AccessToken"] ?? config.AccessToken;
            config.RefreshToken = section["RefreshToken"] ?? config.RefreshToken;
            config.BaseAddress = section["BaseAddress"] ?? config.BaseAddress;
            config.AuthUrl = section["AuthUrl"] ?? config.AuthUrl;
            config.AccessTokenUrl = section["AccessTokenUrl"] ?? config.AccessTokenUrl;

            return services.AddSageActive(opt =>
            {
                opt.Region = config.Region;
                opt.Environment = config.Environment;
                opt.ClientId = config.ClientId;
                opt.ClientSecret = config.ClientSecret;
                opt.SubscriptionKey = config.SubscriptionKey;
                opt.OrganizationId = config.OrganizationId;
                opt.AccessToken = config.AccessToken;
                opt.RefreshToken = config.RefreshToken;
                opt.BaseAddress = config.BaseAddress;
                opt.AuthUrl = config.AuthUrl;
                opt.AccessTokenUrl = config.AccessTokenUrl;
            });
        }
    }
}
