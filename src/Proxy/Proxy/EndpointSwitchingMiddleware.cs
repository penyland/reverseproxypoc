using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using ReverseProxyPOC.Proxy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy
{
    /// <summary>
    /// A middleware that can switch the matched endpoint to a proxy endpoint using feature gates.
    /// </summary>
    public class EndpointSwitchingMiddleware
    {
        private const string ReverseProxyRouteId = "ServiceRoute";
        private readonly RequestDelegate next;
        private readonly IProxyDynamicRoutesConfigurationService apiEndpointConfigurationService;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly IEnumerable<RouteEndpoint> endpoints;

        public EndpointSwitchingMiddleware(
            RequestDelegate next,
            IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService,
            IEnumerable<EndpointDataSource> endpointSources,
            ILogger<EndpointSwitchingMiddleware> logger,
            IConfiguration configuration)
        {
            this.next = next;
            this.apiEndpointConfigurationService = proxyDynamicRoutesConfigurationService;
            this.logger = logger;
            this.configuration = configuration;

            endpoints = endpointSources
                .SelectMany(e => e.Endpoints)
                .OfType<RouteEndpoint>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpoint = (RouteEndpoint)context.GetEndpoint();
            if (endpoint is null)
            {
                return;
            }

            switch (endpoint)
            {
                case RouteEndpoint routeEndpoint:
                    logger.LogInformation($"Endpoint Display Name: {routeEndpoint.DisplayName}");
                    logger.LogInformation($"Endpoint Route Pattern: {routeEndpoint.RoutePattern.RawText}");

                    foreach (var type in routeEndpoint.Metadata.Select(md => md.GetType()))
                    {
                        logger.LogInformation($"{type}");
                    }

                    break;
                case null:
                    break;
            }

            var attribute = endpoint.Metadata.GetMetadata<EndpointFeatureGateAttribute>();
            if (attribute != null)
            {
                IFeatureManager featureManager = context.RequestServices.GetRequiredService<IFeatureManagerSnapshot>();
                var isEnabled = await featureManager.IsEnabledAsync(attribute.Features.First());

                if (!isEnabled)
                {
                    logger.LogInformation($"Endpoint \x1B[1m\x1B[36m'{endpoint.DisplayName}'\x1B[37m is NOT enabled in configuration.");

                    if (attribute.ProxyingAllowed)
                    {
                        logger.LogInformation($"Proxying Allowed for Endpoint \x1B[1m\x1B[36m'{endpoint.DisplayName}.'\x1B[37m");

                        var serviceRouteEndpoint = endpoints.Where(t => t.DisplayName == ReverseProxyRouteId).First();

                        logger.LogInformation($"Changing endpoint to \x1B[1m\x1B[36m'{serviceRouteEndpoint.DisplayName}'\x1B[37m");
                        context.SetEndpoint(serviceRouteEndpoint);
                    }
                    else
                    {
                        logger.LogInformation($"Proxying NOT allowed for Endpoint \x1B[1m\x1B[36m'{endpoint.DisplayName}.'\x1B[37m");
                    //    var disabledFeaturesHandler = context.RequestServices.GetService<IDisabledEndpointHandler>() ?? new DisabledEndpointHandler();
                    //    await disabledFeaturesHandler.HandleDisabledFeatures(attribute.Features, context).ConfigureAwait(false);
                    //    return;
                    }
                }
            }

            await next(context).ConfigureAwait(false);
        }
    }
}
