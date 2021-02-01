using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReverseProxyPOC.Proxy.Models;
using System;
using System.Net.Mime;

namespace ReverseProxyPOC.Proxy.Controllers
{
    /// <summary>
    /// This controller handles events when an Azure App Configuration setting has changed.
    /// </summary>
    [Route("api/updates")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class ConfigurationUpdateController : ControllerBase
    {
        private readonly ILogger<ConfigurationUpdateController> logger;

        public ConfigurationUpdateController(ILogger<ConfigurationUpdateController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult OnConfigurationChangedAsync([FromBody] EventGridEvent<KeyValueModifiedEventData> content)
        {
            this.logger.LogInformation("ConfigurationChanged");

            var eventType = content.EventType switch
            {
                "Microsoft.AppConfiguration.KeyValueModified" => "KeyValueModified",
                "Microsoft.AppConfiguration.KeyValueDeleted" => "KeyValueDeleted",

                _ => throw new ArgumentException(string.Empty)
            };

            return Ok(eventType);
        }
    }
}
