using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Configuration;
using ReverseProxyPOC.Proxy.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReverseProxyPOC.Proxy.Services
{
    public class ProxyDynamicRoutesConfigurationService : IProxyDynamicRoutesConfigurationService
    {
        private readonly IEnumerable<EndpointDataSource> endpointSources;
        private readonly LinkParser linkParser;
        private readonly IConfiguration configuration;
        private readonly DynamicRouteSettings settings;
        private Dictionary<string, object> routes = new Dictionary<string, object>();

        public ProxyDynamicRoutesConfigurationService(
            IEnumerable<EndpointDataSource> endpointSources,
            LinkParser linkParser,
            IConfiguration configuration,
            DynamicRouteSettings settings)
        {
            this.endpointSources = endpointSources ?? throw new System.ArgumentNullException(nameof(endpointSources));
            this.linkParser = linkParser;
            this.configuration = configuration;
            this.settings = settings;

            // TODO: Check if endpointSources are populated. Merge lists

            Initialize();
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

        public EndpointInfo ResolveDynamicEndpoint(string controller, string method, string path)
        {
            var routeEndpoints = endpointSources.SelectMany(e => e.Endpoints).OfType<RouteEndpoint>();

            var routeValues = new RouteValueDictionary();
            string localPath = path; // new Uri(path).LocalPath;

            var matchedEndpoint = routeEndpoints.Where(e => new TemplateMatcher(
                                                                    TemplateParser.Parse(e.RoutePattern.RawText),
                                                                    new RouteValueDictionary())
                                                            .TryMatch(localPath, routeValues))
                                            .OrderBy(c => c.Order)
                                            .FirstOrDefault();

            var all1 = this.settings.Endpoints.Where(t => t.Method == method);
            var all2 = all1.Where(t => t.Controller == controller);
            var all3 = all2.Where(t => t.Route == path);

            // var t = this.settings.Endpoints.Where(t => t.Route == route).FirstOrDefault();
            return all3.FirstOrDefault();
        }

        public void Initialize()
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
                    Route = $"/{e.RoutePattern.RawText.TrimStart('/')}",
                    Action = action,
                    ControllerMethod = controllerMethod
                };
            });

            settings.Endpoints.AddRange(result);
        }

        internal bool IsEnabled() => true;
    }
}
