using Microsoft.Extensions.Primitives;
using Microsoft.ReverseProxy.Abstractions;
using Microsoft.ReverseProxy.Service;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ReverseProxyPOC.Proxy.Services
{
    public sealed class InMemoryProxyConfigProvider : IProxyConfigProvider, IInMemoryProxyConfigProvider, IDisposable
    {
        private volatile InMemoryConfig _config;

        public InMemoryProxyConfigProvider(IReadOnlyList<ProxyRoute> routes, IReadOnlyList<Cluster> clusters)
        {
            _config = new InMemoryConfig(routes, clusters);
        }

        public void Dispose()
        {
            _config.Dispose();
        }

        public IProxyConfig GetConfig() => _config;

        public void Update(IReadOnlyList<ProxyRoute> routes, IReadOnlyList<Cluster> clusters)
        {
            var oldConfig = _config;
            _config = new InMemoryConfig(routes, clusters);
            oldConfig.SignalChange();
        }

        private class InMemoryConfig : IProxyConfig, IDisposable
        {
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();

            public InMemoryConfig(IReadOnlyList<ProxyRoute> routes, IReadOnlyList<Cluster> clusters)
            {
                Routes = routes;
                Clusters = clusters;
                ChangeToken = new CancellationChangeToken(_cts.Token);
            }

            public IReadOnlyList<ProxyRoute> Routes { get; }

            public IReadOnlyList<Cluster> Clusters { get; }

            public IChangeToken ChangeToken { get; }

            public void Dispose()
            {
                _cts.Dispose();
            }

            internal void SignalChange()
            {
                _cts.Cancel();
            }
        }
    }
}
