using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReverseProxyPOC.Proxy.Services;
using System;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Controllers
{
    [Route("proxy")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly IProxyManagerService proxyManagerService;

        public ProxyController(IProxyManagerService proxyManagerService)
        {
            this.proxyManagerService = proxyManagerService ?? throw new ArgumentNullException(nameof(proxyManagerService));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetProxyModeAsync(bool proxyMode)
        {
            await Task.FromResult<object>(null);

            return Ok();
        }
    }
}
