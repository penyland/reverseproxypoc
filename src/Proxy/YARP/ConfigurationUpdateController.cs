using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReverseProxyPOC.Proxy.Models;
using System;
using System.Net.Mime;

namespace ReverseProxyPOC.Proxy.YARP
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

        /// <summary>
        /// [{
        ///  "id": "84e17ea4-66db-4b54-8050-df8f7763f87b",
        ///  "topic": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/testrg/providers/microsoft.appconfiguration/configurationstores/contoso",
        ///  "subject": "https://contoso.azconfig.io/kv/Foo?label=FizzBuzz",
        ///  "data": {
        ///    "key": "Foo",
        ///    "label": "FizzBuzz",
        ///    "etag": "FnUExLaj2moIi4tJX9AXn9sakm0"
        ///  },
        ///  "eventType": "Microsoft.AppConfiguration.KeyValueModified",
        ///  "eventTime": "2019-05-31T20:05:03Z",
        ///  "dataVersion": "1",
        ///  "metadataVersion": "1"
        /// }]
        /// </summary>
        /// <param name="data">The message</param>
        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult OnConfigurationChangedAsync([FromBody] EventGridEvent data)
        {
            this.logger.LogInformation("ConfigurationChanged");

            var eventType = data.EventType switch
            {
                "Microsoft.AppConfiguration.KeyValueModified" => "KeyValueModified",
                "Microsoft.AppConfiguration.KeyValueDeleted" => "KeyValueDeleted",

                _ => throw new ArgumentException(string.Empty)
            };

            return Ok(eventType);
        }

        private T GetData<T>(EventGridEvent eventGridEvent)
        {
            return (T)eventGridEvent.Data;
        }
    }
}
