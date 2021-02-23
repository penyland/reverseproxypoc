using Microsoft.AspNetCore.Builder;
using ReverseProxyPOC.Proxy.Proxy;

namespace ReverseProxyPOC.Proxy
{
    public static class EndpointSwitchingExtensions
    {
        public static IApplicationBuilder UseFeatureGatedEndpointSwitching(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EndpointSwitchingMiddleware>();
        }
    }
}
