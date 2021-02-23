using Microsoft.AspNetCore.Builder;

namespace ReverseProxyPOC.Proxy.Proxy.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder RequireFeature(this IEndpointConventionBuilder builder, string featureName)
        {
            builder.Add(endpointBuilder =>
            {
                var featureGate = new EndpointFeatureGateAttribute(true, featureName);

                endpointBuilder.Metadata.Add(featureGate);
            });

            return builder;
        }
    }
}
