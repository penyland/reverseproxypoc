using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
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
        private readonly IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService;
        private readonly IEnumerable<EndpointDataSource> endpointSources;
        private readonly ILogger logger;
        private readonly List<RouteEndpoint> endpoints;

        public ProxyDynamicEndpointsMiddleware(
            RequestDelegate next,
            IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService,
            IEnumerable<EndpointDataSource> endpointSources,
            ILogger<ProxyDynamicEndpointsMiddleware> logger)
        {
            this.next = next;
            this.proxyDynamicRoutesConfigurationService = proxyDynamicRoutesConfigurationService;
            this.endpointSources = endpointSources;
            this.logger = logger;

            endpoints = endpointSources
                .SelectMany(e => e.Endpoints)
                .OfType<RouteEndpoint>()
                .ToList();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpoint = context.GetEndpoint();
            if (endpoint is null)
            {
                return;
            }

            var selectedEndpoint = endpoints.Where(t => t.DisplayName == context.GetEndpoint().DisplayName).First();

            if (!proxyDynamicRoutesConfigurationService.IsEnabled(selectedEndpoint.DisplayName))
            {
                logger.LogInformation($"Endpoint \x1B[1m\x1B[36m'{selectedEndpoint.DisplayName}'\x1B[37m is not enabled.");

                var yarpEndpoint = endpoints.Where(t => t.DisplayName == ReverseProxyRouteId).First();
                logger.LogInformation($"Changing endpoint to \x1B[1m\x1B[36m'{yarpEndpoint.DisplayName}'\x1B[37m");

                context.SetEndpoint(yarpEndpoint);
            }

            await next(context);
        }
    }
}
