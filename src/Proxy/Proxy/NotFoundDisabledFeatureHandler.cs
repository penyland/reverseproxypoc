// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.FeatureManagement.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy
{
    /// <summary>
    /// A default disabled feature handler that performs a minimal amount of work for disabled feature requests.
    /// </summary>
    public class NotFoundDisabledFeaturesHandler : IDisabledFeaturesHandler
    {
        public Task HandleDisabledFeatures(IEnumerable<string> features, ActionExecutingContext context)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status404NotFound);

            return Task.CompletedTask;
        }
    }
}
