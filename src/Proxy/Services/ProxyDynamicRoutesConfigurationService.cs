using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using ReverseProxyPOC.Proxy.Proxy;
using System.Collections.Generic;
using System.Linq;

namespace ReverseProxyPOC.Proxy.Services
{
    public class ProxyDynamicRoutesConfigurationService : IProxyDynamicRoutesConfigurationService
    {
        private readonly IEnumerable<EndpointDataSource> endpointSources;
        private readonly DynamicRouteSettings settings;
        private Dictionary<string, EndpointInfo> routes = new Dictionary<string, EndpointInfo>();

        public ProxyDynamicRoutesConfigurationService(
            IEnumerable<EndpointDataSource> endpointSources,
            DynamicRouteSettings settings)
        {
            this.endpointSources = endpointSources ?? throw new System.ArgumentNullException(nameof(endpointSources));
            this.settings = settings;

            Initialize();
        }

        public bool IsEnabled(string routeName)
        {
            if (routes.TryGetValue(routeName, out var value))
            {
                return value.IsEnabled;
            }
            else
            {
                return false;
            }
        }

        private void Initialize()
        {
            var endpoints = endpointSources
                .SelectMany(e => e.Endpoints)
                .OfType<RouteEndpoint>();

            var result = endpoints.Select(e =>
            {
                var controller = e.Metadata
                    .OfType<ControllerActionDescriptor>()
                    .FirstOrDefault();
                var action = controller != null
                    ? $"{controller.ControllerName}.{controller.ActionName}"
                    : null;
                var controllerMethod = controller != null
                    ? $"{controller.ControllerTypeInfo.FullName}:{controller.MethodInfo.Name}"
                    : null;

                return new EndpointInfo()
                {
                    Order = e.Order,
                    Controller = controller?.ControllerName ?? string.Empty,
                    Method = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
                    RoutePattern = $"/{e.RoutePattern.RawText.TrimStart('/')}",
                    Action = action,
                    ControllerMethod = controllerMethod,
                    DisplayName = e.DisplayName
                };
            });

            routes = result.ToDictionary(t => t.DisplayName, t => t);

            foreach (var route in settings.Endpoints)
            {
                if (routes.ContainsKey(route.DisplayName))
                {
                    routes[route.DisplayName] = route;
                }
            }
        }
    }
}
