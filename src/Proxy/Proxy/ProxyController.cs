using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ReverseProxy.Service;
using ReverseProxyPOC.Proxy.Proxy;
using ReverseProxyPOC.Proxy.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.YARP
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ProxyController : ControllerBase
    {
        private readonly IProxyDynamicRoutesConfigurationService proxyConfigurationService;
        private readonly IProxyConfigProvider proxyConfigProvider;
        private readonly IConfigurationRoot configuration;

        public ProxyController(IProxyDynamicRoutesConfigurationService proxyConfigurationService, IProxyConfigProvider proxyConfigProvider, IConfiguration configuration)
        {
            this.proxyConfigurationService = proxyConfigurationService ?? throw new ArgumentNullException(nameof(proxyConfigurationService));
            this.proxyConfigProvider = proxyConfigProvider ?? throw new ArgumentNullException(nameof(proxyConfigProvider));
            this.configuration = (IConfigurationRoot)configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult Get()
        {
            var config = this.proxyConfigProvider.GetConfig();
            return Ok(config);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetProxyModeAsync(bool proxyMode)
        {
            var currentConfiguration = this.configuration;
            var reverseProxySection = currentConfiguration.GetSection("ReverseProxy");
            var routes = reverseProxySection.GetSection("Routes");
            var path = routes.GetValue<string>("0:Match:Path");

            if (proxyMode)
            {
                this.configuration.GetSection("ReverseProxy:Routes:0:Match:Path").Value = "/proxy/";
                configuration.Reload();
            }

            var config = this.proxyConfigProvider.GetConfig();
            var token = config.ChangeToken;

            await Task.FromResult<object>(null);

            return Ok(config);
        }

        [HttpGet("endpoints")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EndpointInfo>))]
        public IActionResult GetEndpoints()
        {
            return Ok(this.proxyConfigurationService.Endpoints);
        }
    }
}
