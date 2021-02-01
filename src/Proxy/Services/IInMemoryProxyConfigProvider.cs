using Microsoft.ReverseProxy.Abstractions;
using System.Collections.Generic;

namespace ReverseProxyPOC.Proxy.Services
{
    public interface IInMemoryProxyConfigProvider
    {
        void Update(IReadOnlyList<ProxyRoute> routes, IReadOnlyList<Cluster> clusters);
    }
}