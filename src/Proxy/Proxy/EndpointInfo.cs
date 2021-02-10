namespace ReverseProxyPOC.Proxy.Proxy
{
    public class EndpointInfo
    {
        public string Controller { get; set; }

        public string Method { get; set; }

        public string Action { get; set; }

        public int Order { get; set; }

        public string Route { get; set; }

        public string ControllerMethod { get; set; }

        public string FeatureFlag { get; set; }

        public bool IsEnabled { get; set; }

        public string DisplayName { get; internal set; }
    }
}
