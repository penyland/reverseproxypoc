using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Frontend
{
    public class ProxyHttpClient
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly HttpClient client;

        public ProxyHttpClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task SetProxyModeAsync(bool proxyRequests)
        {
            var response = await this.client.PostAsync("/proxy", new StringContent(JsonSerializer.Serialize(proxyRequests)));
        }

        public async Task<WeatherForecast[]> GetWeatherAsync()
        {
            var responseMessage = await this.client.GetAsync("/weatherforecast");
            var stream = await responseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<WeatherForecast[]>(stream, options);
        }
    }
}
