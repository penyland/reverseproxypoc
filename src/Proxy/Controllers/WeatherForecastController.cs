using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using ReverseProxyPOC.Proxy.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReverseProxyPOC.Proxy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        //[EndpointFeatureGate(features: FeatureFlags.WeatherForecasts)]
        [EndpointFeatureGate(Microsoft.FeatureManagement.RequirementType.All, true, FeatureFlags.ProxyingAllowed, FeatureFlags.WeatherForecasts)]
        public IEnumerable<WeatherForecast> GetForecasts()
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

        [HttpGet("{id:int}")]
        //[EndpointFeatureGate(features: FeatureFlags.WeatherForecast)]
        public WeatherForecast GetForecast(int id)
        {
            var rng = new Random();
            return new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };
        }
    }
}
