using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ReverseProxy.Service.Proxy;
using System;

namespace ReverseProxyPOC.Proxy.YARP
{
    public class ProxyAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // IHostEnvironment
            var env = context.HttpContext.RequestServices.GetService<IHostEnvironment>();
            if (!env.IsDevelopment())
            {
                context.Result = new NotFoundResult();
            }

            var httpProxy = context.HttpContext.RequestServices.GetService<IHttpProxy>();
        }
    }
}
