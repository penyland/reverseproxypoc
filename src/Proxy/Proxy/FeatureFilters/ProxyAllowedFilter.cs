using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy.Filters
{
    public class ProxyAllowedFilter : IFeatureFilter
    {
        public ProxyAllowedFilter()
        {
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var settings = context.Parameters.Get<ProxyAllowedFilterSettings>() ?? new ProxyAllowedFilterSettings();

            if (settings.ProxyingAllowed)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
