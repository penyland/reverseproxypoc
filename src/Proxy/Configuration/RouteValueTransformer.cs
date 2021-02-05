using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using ReverseProxyPOC.Proxy.Services;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Configuration
{
    public class RouteValueTransformer : DynamicRouteValueTransformer
    {
        private readonly IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService;

        public RouteValueTransformer(IProxyDynamicRoutesConfigurationService proxyDynamicRoutesConfigurationService)
        {
            this.proxyDynamicRoutesConfigurationService = proxyDynamicRoutesConfigurationService ?? throw new System.ArgumentNullException(nameof(proxyDynamicRoutesConfigurationService));
        }

        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (!values.ContainsKey("controller"))
            {
                return new ValueTask<RouteValueDictionary>(values);
            }

            var controller = values["controller"];

            // Check if endpoint is enabled in configuration or not
            // if (!endPointEnabled(controller))
            // {
            //    return new ValueTask<RouteValueDictionary>(values);
            // }

            if (values.ContainsKey("controller") && values.ContainsKey("id"))
            {
                // Get controller and action from configuration service
                values["controller"] = "WeatherForecast";
                values["action"] = "GetForecast";
            }
            else
            {
                var endpointInfo = proxyDynamicRoutesConfigurationService.GetController((string)values["controller"]);

                values["controller"] = endpointInfo.Controller; // <= must be the name of the controller not route
                values["action"] = endpointInfo.Action; // <= method name that we are calling
            }

            return new ValueTask<RouteValueDictionary>(values);
        }
    }
}
