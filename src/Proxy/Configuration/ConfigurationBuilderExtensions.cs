using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddCustomConfiguration(
            this IConfigurationBuilder builder, Action action)
        {
            builder.Add(new CustomConfigurationSource(action));

            return builder;
        }
    }
}
