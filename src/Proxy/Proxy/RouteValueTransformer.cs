using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ReverseProxy.Middleware;
using ReverseProxyPOC.Proxy.Services;
using Serilog;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Configuration
{
    public class RouteValueTransformer : DynamicRouteValueTransformer
    {
        private readonly IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService;
        private readonly ILogger<RouteValueTransformer> logger;

        public RouteValueTransformer(IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService, ILogger<RouteValueTransformer> logger)
        {
            this.proxyDynamicRoutesConfigurationService = proxyDynamicRoutesConfigurationService ?? throw new System.ArgumentNullException(nameof(proxyDynamicRoutesConfigurationService));
            this.logger = logger;
        }

        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            using (logger.BeginScope("RouteValueTransformer"))
            {
                if (!values.ContainsKey("controller"))
                {
                    return new ValueTask<RouteValueDictionary>(values);
                }

                foreach (var value in values)
                {
                    logger.LogInformation("{0}, {1}", value.Key, value.Value);
                }

                var components = httpContext.RequestServices.GetServices<IHttpRequestFeature>();

                var routeData = httpContext.GetRouteData();
                var httpRequestFeature = httpContext.Features.Get<IHttpRequestFeature>();
                var routeValuesFeature = httpContext.Features.Get<IRouteValuesFeature>();
                var routesValues = routeValuesFeature.RouteValues;

                var rp = httpContext.Features.Get<IReverseProxyFeature>();

                var controller = values["controller"] as string;
                var id = values["id"];
                var action = values["action"] as string;
                var method = httpRequestFeature.Method;
                var path = httpRequestFeature.Path;

                var endpoint = proxyDynamicRoutesConfigurationService.ResolveDynamicEndpoint(controller, method, path);

                if (endpoint == null)
                {
                    values["controller"] = string.Empty;
                    values["action"] = string.Empty;
                    values["id"] = string.Empty;
                    return new ValueTask<RouteValueDictionary>(values);
                }
                else
                {
                    values["controller"] = endpoint.Controller; // <= must be the name of the controller not route
                    values["action"] = endpoint.Action; // <= method name that we are calling
                }

                // Check if endpoint is enabled in configuration or not
                // if (!endPointEnabled(controller))
                // {
                //    return new ValueTask<RouteValueDictionary>(values);
                // }

                //if (values.ContainsKey("controller") && values.ContainsKey("id"))
                //{
                //    // Get controller and action from configuration service
                //    values["controller"] = "WeatherForecast";
                //    values["action"] = "GetForecast";
                //}
                //else
                //{
                //    var endpointInfo = proxyDynamicRoutesConfigurationService.GetController((string)values["controller"]);

                //    values["controller"] = endpointInfo.Controller; // <= must be the name of the controller not route
                //    values["action"] = endpointInfo.Action; // <= method name that we are calling
                //}

                return new ValueTask<RouteValueDictionary>(values);
            }
        }
    }
}
