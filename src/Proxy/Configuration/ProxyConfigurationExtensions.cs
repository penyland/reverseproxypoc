using Microsoft.AspNetCore.Builder;
using ReverseProxyPOC.Proxy.Services;
using System;

namespace ReverseProxyPOC.Proxy
{
    public static class ProxyConfigurationExtensions
    {
        /// <summary>
        /// Configures a middleware for Proxy Configuration to use activity-based refresh for data configured in the provider.
        /// </summary>
        /// <param name="builder">An instance of <see cref="IApplicationBuilder"/></param>
        public static IApplicationBuilder UseProxyConfigurationRefresher(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // Verify if IProxyConfigurationService was done before calling UseProxyConfigurationRefresher.
            // We use the IProxyConfigurationService to make sure if the required services were added.
            if (builder.ApplicationServices.GetService(typeof(IProxyDynamicRoutesConfigurationService)) == null)
            {
                throw new InvalidOperationException("Unable to find the required services. Please add all the required services by calling 'IServiceCollection.AddProxyConfiguration' inside the call to 'ConfigureServices(...)' in the application startup code.");
            }

            return builder.UseMiddleware<ProxyConfigurationRefreshMiddleware>();
        }
    }
}
