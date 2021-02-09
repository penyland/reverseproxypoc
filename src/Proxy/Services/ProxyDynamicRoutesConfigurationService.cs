using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace ReverseProxyPOC.Proxy.Services
{
    public class ProxyDynamicRoutesConfigurationService : IProxyDynamicRoutesConfigurationService
    {
        private readonly IEnumerable<EndpointDataSource> endpointSources;
        private Dictionary<string, object> routes = new Dictionary<string, object>();

        public ProxyDynamicRoutesConfigurationService(IEnumerable<EndpointDataSource> endpointSources)
        {
            this.endpointSources = endpointSources ?? throw new System.ArgumentNullException(nameof(endpointSources));
        }

        public (string Controller, string Action) GetController(string route)
        {
            return route switch
            {
                "WeatherForecast" => ("WeatherForecast", "GetForecasts"),
                "WeatherForecast/2" => ("WeatherForecast", "GetForecast"),
                "api/WeatherForecast" => ("WeatherForecast", "GetForecast"),
                "api/proxy" => ("Proxy", "Get"),
                "Todo" => ("Todo", "GetTodoItems"),

                _ => (route, string.Empty)
            };
        }

        public void Initialize()
        {
            var endpoints = endpointSources
                .SelectMany(e => e.Endpoints)
                .OfType<RouteEndpoint>();

            //var endpoint1 = endpoints.ToList()[1];
            //var t = endpoint1.Metadata.OfType<ControllerActionDescriptor>().ToList();

            var result = endpoints.Select(t =>
            {
                var controller = t.Metadata
                    .OfType<ControllerActionDescriptor>()
                    .FirstOrDefault();
                var action = controller != null
                    ? $"{controller.ActionName}"
                    : null;
                var controllerMethod = controller != null
                    ? $"{controller.ControllerTypeInfo.FullName}:{controller.MethodInfo.Name}"
                    : null;
                return new
                {
                    DisplayName = t.DisplayName,
                    Order = t.Order,
                    Controller = controller?.ControllerName ?? string.Empty,
                    Method = t.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
                    Route = $"/{t.RoutePattern.RawText.TrimStart('/')}",
                    Action = action,
                    ControllerMethod = controllerMethod
                };
            });
        }
    }
}
