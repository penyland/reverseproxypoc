using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReverseProxyPOC.Proxy.Configuration;
using ReverseProxyPOC.Proxy.Services;
using System;

namespace ReverseProxyPOC.Proxy.Proxy
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureProxy(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddOptions<DynamicRouteSettings>().Configure<IConfiguration>((settings, config) => configuration.GetSection("Endpoints").Bind(settings));

            //foreach (var section in configuration.GetSection("Endpoints").GetChildren())
            //{
            //}

            services.AddSingleton<IProxyDynamicRoutesConfigurationService, ProxyDynamicRoutesConfigurationService>();
            services.AddSingleton<RouteValueTransformer>();

            return services;
        }

        public static TSettings AddConfig<TSettings>(this IServiceCollection services, IConfiguration configuration)
            where TSettings : class, new()
        {
            return services.AddConfig<TSettings>(configuration, options => { });
        }

        public static TSettings AddConfig<TSettings>(this IServiceCollection services, IConfiguration configuration, Action<BinderOptions> configureOptions)
            where TSettings : class, new()
        {
            var setting = configuration.Get<TSettings>(configureOptions);
            services.TryAddSingleton(setting);
            return setting;
        }
    }
}
