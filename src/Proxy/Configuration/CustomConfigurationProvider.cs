using Microsoft.Extensions.Configuration;
using System;

namespace ReverseProxyPOC.Proxy.Configuration
{
    public class CustomConfigurationProvider : ConfigurationProvider
    {
        private readonly Action action;

        public CustomConfigurationProvider(Action action)
        {
            this.action = action;
        }

        public override void Load()
        {
            action();
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);
            Console.WriteLine("Set: {0} to {1}", key, value);
        }
    }
}
