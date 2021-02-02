using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ReverseProxy.Service.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReverseProxyPOC.Proxy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Proxy Freezing", "Proxy Bracing", "Proxy Chilly", "Proxy Cool", "Proxy Mild", "Proxy Warm", "Proxy Balmy", "Proxy Hot", "Proxy Sweltering", "Proxy Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpProxy httpProxy;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpProxy httpProxy)
        {
            _logger = logger;
            this.httpProxy = httpProxy;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("GetWeatherForecasts");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
