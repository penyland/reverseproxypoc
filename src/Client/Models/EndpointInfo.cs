namespace ReverseProxyPOC.Client.Models
{
    public record EndpointInfo
    {
        public string Controller { get; init; }

        public string Method { get; init; }

        public string Action { get; init; }

        public int Order { get; init; }

        public string RoutePattern { get; init; }

        public string ControllerMethod { get; init; }

        public string FeatureFlag { get; set; }

        public bool IsEnabled { get; set; }

        public string DisplayName { get; init; }
    }
}
