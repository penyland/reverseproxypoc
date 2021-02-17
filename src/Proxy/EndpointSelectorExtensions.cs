using Microsoft.AspNetCore.Builder;
using ReverseProxyPOC.Proxy.Proxy;

namespace ReverseProxyPOC.Proxy
{
    public static class EndpointSelectorExtensions
    {
        public static IApplicationBuilder UseEndpointSelector(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ProxyDynamicEndpointsMiddleware>();
        }
    }
}
