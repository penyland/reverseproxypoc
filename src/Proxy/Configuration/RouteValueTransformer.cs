using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using ReverseProxyPOC.Proxy.Services;
using System.Collections.Generic;
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
            //var url = httpContext.Request.Path.ToString();

            //if (!values.ContainsKey("controller"))
            //{
            //    return new ValueTask<RouteValueDictionary>(values);
            //}

            if (values.ContainsKey("controller") && values.ContainsKey("id"))
            {
                values["controller"] = "WeatherForecast";
                values["action"] = "GetForecast";
            }
            else
            {
                var value = proxyDynamicRoutesConfigurationService.GetController((string)values["controller"]);

                values["controller"] = value.Controller;
                values["action"] = value.Action; // <= method name that we are calling
            }

            //// values["controller"] => must be the name of the controller not route
            //values["id"] = "id";

            return new ValueTask<RouteValueDictionary>(values);
        }

        public override ValueTask<IReadOnlyList<Endpoint>> FilterAsync(HttpContext httpContext, RouteValueDictionary values, IReadOnlyList<Endpoint> endpoints)
        {
            return base.FilterAsync(httpContext, values, endpoints);
        }
    }
}
