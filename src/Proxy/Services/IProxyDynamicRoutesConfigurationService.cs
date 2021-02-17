using Microsoft.AspNetCore.Routing;
using ReverseProxyPOC.Proxy.Proxy;
using System.Collections.Generic;

namespace ReverseProxyPOC.Proxy.Services
{
    public interface IProxyDynamicRoutesConfigurationService
    {
        IEnumerable<EndpointInfo> Endpoints { get; set; }

        bool IsEnabled(RouteEndpoint routeEndpoint);
    }
}
