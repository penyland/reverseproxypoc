using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Frontend.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly WeatherClient weatherClient;
        private readonly ProxyHttpClient proxyHttpClient;

        public IndexModel(ILogger<IndexModel> logger, WeatherClient weatherClient, ProxyHttpClient proxyHttpClient)
        {
            _logger = logger;
            this.weatherClient = weatherClient;
            this.proxyHttpClient = proxyHttpClient ?? throw new System.ArgumentNullException(nameof(proxyHttpClient));
            Forecasts = new WeatherForecast[] { };
        }

        public bool UseProxy { get; set; } = true;

        public string Message { get; set; }

        public WeatherForecast[] Forecasts { get; set; }

        public void OnGet()
        {
            Message = "Proxy mode";
        }

        public void OnPost()
        {
            Message = "Post used";
        }

        public async Task OnPostProxyAsync(bool useProxy)
        {
            UseProxy = useProxy;
            if (useProxy)
            {
                Message = "Proxy mode";
            }
            else
            {
                Message = "Direct mode";
            }

            await this.proxyHttpClient.SetProxyModeAsync(useProxy);
        }

        public void OnPostDirect()
        {
            Message = "Using direct mode";
        }

        public async Task OnPostApiAsync()
        {
            Forecasts = await weatherClient.GetWeatherAsync();
        }
    }
}
