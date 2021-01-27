using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ReverseProxyPOC.Frontend;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Pages
{
    public class WeatherModel : PageModel
    {
        private readonly ILogger<WeatherModel> _logger;

        public WeatherModel(ILogger<WeatherModel> logger)
        {
            _logger = logger;
        }

        public WeatherForecast[] Forecasts { get; set; }

        public async Task OnGetAsync([FromServices] WeatherClient client)
        {
            Forecasts = await client.GetWeatherAsync();
        }
    }
}
