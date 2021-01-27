using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy
{
    public class ProxyConfigurationRefreshMiddleware
    {
        private readonly RequestDelegate next;

        public ProxyConfigurationRefreshMiddleware(RequestDelegate requestDelegate)
        {
            next = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await next(context).ConfigureAwait(false);
        }
    }
}
