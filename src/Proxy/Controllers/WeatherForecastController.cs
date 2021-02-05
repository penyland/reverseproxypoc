using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
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

        [HttpGet("{id?}")]
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
