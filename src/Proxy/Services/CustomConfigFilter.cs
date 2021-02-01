using Microsoft.ReverseProxy.Abstractions;
using Microsoft.ReverseProxy.Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Services
{
    public class CustomConfigFilter : IProxyConfigFilter
    {
        public async Task ConfigureClusterAsync(Cluster cluster, CancellationToken cancel)
        {
           await Task.FromResult<object>(null);
        }

        public async Task ConfigureRouteAsync(ProxyRoute route, CancellationToken cancel)
        {
            await Task.FromResult<object>(null);
        }
    }
}
