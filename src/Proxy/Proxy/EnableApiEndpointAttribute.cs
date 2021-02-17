using System;

namespace ReverseProxyPOC.Proxy.Proxy
{
    /// <summary>
    /// An attribute that can be placed on MVC actions to require the endpoint to be enabled or not.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class EnableApiEndpointAttribute : Attribute
    {
        // EndpointIsEnabledAttribute
        // Endpoint
        // APIProxyAttribute
        // ConditionalEndpointAttribute
        // ConditionalOnFeatureEndpointAttribute
        // FeatureGateAttribute
        // EndpointEnabledFeatureAttribute
        // EnableApiEndpointAttribute
        // AllowApiEndpointThroughProxyAttribute
        // Route
        // AllowRoute
        // ProxyRouteSettingsAttribute
        // ProxyRouteFeatureAttribute
        // ProxyRouteEndpoint
        // RouteEnabledSelector

        /// <summary>
        /// An attribute that can be placed on Controllers and actions to indicate if an endpoint should be executed or proxied.
        /// </summary>
        /// <param name="isEnabled">True if the endpoint is implemented and selectable. False to proxy to old backend service.</param>
        public EnableApiEndpointAttribute(bool isEnabled = true)
        {
            this.IsEnabled = isEnabled;
        }

        public bool IsEnabled { get; }
    }
}
