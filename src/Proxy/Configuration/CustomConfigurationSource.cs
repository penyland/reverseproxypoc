using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace ReverseProxyPOC.Proxy.Configuration
{
    public class CustomConfigurationSource : IConfigurationSource
    {
        private readonly Action action;

        public CustomConfigurationSource(Action action)
        {
            this.action = action;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new CustomConfigurationProvider(action);
    }
}
