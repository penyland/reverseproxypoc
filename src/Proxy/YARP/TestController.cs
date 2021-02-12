using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using ReverseProxyPOC.Proxy.Proxy;
using System.Collections.Generic;
using System.Linq;

namespace ReverseProxyPOC.Proxy.YARP
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;
        private readonly IEnumerable<EndpointDataSource> endpointSources;

        public TestController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IEnumerable<EndpointDataSource> endpointSources)
        {
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            this.endpointSources = endpointSources;
        }

        [HttpGet("actions")]
        public IActionResult GetActions()
        {
            return Ok(actionDescriptorCollectionProvider
                .ActionDescriptors
                .Items
                .OfType<ControllerActionDescriptor>()
                .Select(a => new
                {
                    a.DisplayName,
                    a.ControllerName,
                    a.ActionName,
                    AttributeRouteTemplate = a.AttributeRouteInfo?.Template,
                    HttpMethods = string.Join(", ", a.ActionConstraints?.OfType<HttpMethodActionConstraint>().SingleOrDefault()?.HttpMethods ?? new string[] { "any" }),
                    Parameters = a.Parameters?.Select(p => new
                    {
                        Type = p.ParameterType.Name,
                        p.Name
                    }),
                    ControllerClassName = a.ControllerTypeInfo.FullName,
                    ActionMethodName = a.MethodInfo.Name,
                    Filters = a.FilterDescriptors?.Select(f => new
                    {
                        ClassName = f.Filter.GetType().FullName,
                        f.Scope
                    }),
                    Constraints = a.ActionConstraints?.Select(c => new
                    {
                        Type = c.GetType().Name
                    }),
                    RouteValues = a.RouteValues.Select(r => new
                    {
                        r.Key,
                        r.Value
                    }),
                }));
        }

        [HttpGet("pages")]
        public IActionResult GetPages()
        {
            return Ok(actionDescriptorCollectionProvider
                .ActionDescriptors
                .Items
                .OfType<PageActionDescriptor>()
                .Select(a => new
                {
                    a.DisplayName,
                    a.ViewEnginePath,
                    a.RelativePath,
                }));
        }

        [HttpGet("routes")]
        public IActionResult GetRoutes()
        {
            var endpoints = endpointSources
                .SelectMany(e => e.Endpoints)
                .OfType<RouteEndpoint>();

            var endpoint1 = endpoints.ToList()[1];
            var t = endpoint1.Metadata.OfType<ControllerActionDescriptor>().ToList();

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
                return new EndpointInfo
                {
                    DisplayName = t.DisplayName,
                    Order = t.Order,
                    Controller = controller?.ControllerName ?? string.Empty,
                    Method = t.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
                    RoutePattern = $"/{t.RoutePattern.RawText.TrimStart('/')}",
                    Action = action,
                    ControllerMethod = controllerMethod
                };
            });

            return Ok(result);
        }

    }
}
