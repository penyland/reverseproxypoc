using System.Collections.Generic;

namespace ReverseProxyPOC.Proxy.Proxy
{
    public class DynamicRouteSettings
    {
        public DynamicRouteSettings()
        {
        }

        public List<EndpointInfo> Endpoints { get; } = new List<EndpointInfo>();
    }
}
