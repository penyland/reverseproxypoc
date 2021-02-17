using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ReverseProxy.Middleware;
using ReverseProxyPOC.Proxy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy
{
    public class ProxyDynamicEndpointsMiddleware
    {
        private const string ReverseProxyRouteId = "ServiceRoute";
        private readonly RequestDelegate next;
        private readonly IProxyDynamicRoutesConfigurationService apiEndpointConfigurationService;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly IEnumerable<RouteEndpoint> endpoints;

        public ProxyDynamicEndpointsMiddleware(
            RequestDelegate next,
            IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService,
            IEnumerable<EndpointDataSource> endpointSources,
            ILogger<ProxyDynamicEndpointsMiddleware> logger,
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

        public async Task Invoke(HttpContext context)
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

            // Make sure reverse proxy is added
            var routeId = configuration["ReverseProxy:Routes:0:RouteId"];

            var attribute = endpoint.Metadata.GetMetadata<EnableApiEndpointAttribute>();
            if (attribute != null)
            {
                var isEnabled = attribute.IsEnabled;
                logger.LogInformation($"Endpoint \x1B[1m\x1B[36m'{endpoint.DisplayName}'\x1B[37m is NOT enabled.");

                if (!apiEndpointConfigurationService.IsEnabled(endpoint))
                {
                    logger.LogInformation($"Endpoint \x1B[1m\x1B[36m'{endpoint.DisplayName}'\x1B[37m is NOT enabled in configuration.");

                    var serviceRouteEndpoint = endpoints.Where(t => t.DisplayName == ReverseProxyRouteId).First();
                    logger.LogInformation($"Changing endpoint to \x1B[1m\x1B[36m'{serviceRouteEndpoint.DisplayName}'\x1B[37m");

                    context.SetEndpoint(serviceRouteEndpoint);
                }
            }

            await next(context);
        }
    }
}
