using Microsoft.Extensions.DependencyInjection;
using ReverseProxyPOC.Proxy.Services;
using System;

namespace ReverseProxyPOC.Proxy
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddProxyConfiguration(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IProxyConfigurationService, ProxyConfigurationService>();
            return services;
        }
    }
}
