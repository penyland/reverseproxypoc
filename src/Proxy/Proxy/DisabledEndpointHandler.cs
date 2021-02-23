using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.FeatureManagement.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy
{
    public class DisabledEndpointHandler : IDisabledEndpointHandler, IDisabledFeaturesHandler
    {
        public Task HandleDisabledFeatures(IEnumerable<string> features, HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            return Task.CompletedTask;
        }

        public Task HandleDisabledFeatures(IEnumerable<string> features, ActionExecutingContext context)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);

            return Task.CompletedTask;
        }
    }
}
