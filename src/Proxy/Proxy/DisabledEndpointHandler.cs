using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy
{
    public class DisabledEndpointHandler : IDisabledEndpointHandler
    {
        public Task HandleDisabledFeatures(IEnumerable<string> features, HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            return Task.CompletedTask;
        }
    }
}
