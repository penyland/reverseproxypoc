using Microsoft.ReverseProxy.Abstractions;
using System.Collections.Generic;

namespace ReverseProxyPOC.Proxy
{
    internal class InMemoryConfigProvider
    {
        private IReadOnlyList<ProxyRoute> routes;
        private IReadOnlyList<Cluster> clusters;

        public InMemoryConfigProvider(IReadOnlyList<ProxyRoute> routes, IReadOnlyList<Cluster> clusters)
        {
            this.routes = routes;
            this.clusters = clusters;
        }
    }
}