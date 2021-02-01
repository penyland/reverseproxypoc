using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.ReverseProxy.Service;
using ReverseProxyPOC.Proxy.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Controllers
{
    [Route("proxy")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly IProxyConfigurationService proxyConfigurationService;
        private readonly IProxyConfigProvider proxyConfigProvider;
        private readonly IConfigurationRoot configuration;

        public ProxyController(IProxyConfigurationService proxyConfigurationService, IProxyConfigProvider proxyConfigProvider, IConfiguration configuration)
        {
            this.proxyConfigurationService = proxyConfigurationService ?? throw new ArgumentNullException(nameof(proxyConfigurationService));
            this.proxyConfigProvider = proxyConfigProvider ?? throw new ArgumentNullException(nameof(proxyConfigProvider));
            this.configuration = (IConfigurationRoot)configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public ProxyController(IOptionsMonitor<IProxyConfigProvider> optionsMonitor)
        {
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
    }
}
