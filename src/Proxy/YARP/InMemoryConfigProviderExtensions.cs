using Microsoft.Extensions.DependencyInjection;
using Microsoft.ReverseProxy.Abstractions;
using Microsoft.ReverseProxy.Service;
using ReverseProxyPOC.Proxy.Services;
using System.Collections.Generic;

namespace ReverseProxyPOC.Proxy
{
    public static class InMemoryConfigProviderExtensions
    {
        public static IReverseProxyBuilder LoadFromMemory(this IReverseProxyBuilder builder, IReadOnlyList<ProxyRoute> routes, IReadOnlyList<Cluster> clusters)
        {
            builder.Services.AddSingleton<IProxyConfigProvider>(new InMemoryProxyConfigProvider(routes, clusters));
            return builder;
        }
    }
}
