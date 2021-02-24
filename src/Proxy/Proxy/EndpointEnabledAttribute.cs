using System;

namespace ReverseProxyPOC.Proxy.Proxy
{
    /// <summary>
    /// An attribute that can be placed on controllers and actions to require all or any of a set of features to be enabled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class EndpointEnabledAttribute : Attribute
    {
        // Add support for reading from configuration
        public EndpointEnabledAttribute(bool proxyingAllowed = true, bool enabled = true)
        {
            this.IsEnabled = enabled;
            this.ProxyingAllowed = proxyingAllowed;
        }

        public bool IsEnabled { get; }

        public bool ProxyingAllowed { get; }
    }
}
