using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy
{
    /// <summary>
    /// A handler that is invoked when an endpoint requires a feature and the feature is disabled
    /// </summary>
    public interface IDisabledEndpointHandler
    {
        /// <summary>
        /// Callback used to handle requests to an endpoint that require a feature that is disabled.
        /// </summary>
        /// <param name="features">The name of the features that the action could have been activated for.</param>
        /// <param name="context">The action executing context provided by MVC.</param>
        /// <returns>The task.</returns>
        Task HandleDisabledFeatures(IEnumerable<string> features, HttpContext context);
    }
}
