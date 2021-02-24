using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy.Extensions
{
    public static class EndpointSwitchingAppBuilderExtensions
    {
        public static IApplicationBuilder UseEndpointSwitching(this IApplicationBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            VerifyServicesRegistered
        }
    }
}
