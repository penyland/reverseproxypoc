using Microsoft.Extensions.Hosting;
using ReverseProxyPOC.Proxy.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy
{
    public class RoutesRepositoryHostedService : IHostedService
    {
        private readonly IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService;

        public RoutesRepositoryHostedService(IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService)
        {
            this.proxyDynamicRoutesConfigurationService = proxyDynamicRoutesConfigurationService ?? throw new ArgumentNullException(nameof(proxyDynamicRoutesConfigurationService));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            proxyDynamicRoutesConfigurationService.Initialize();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
