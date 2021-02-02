namespace ReverseProxyPOC.Proxy.Services
{
    public class ProxyDynamicRoutesConfigurationService : IProxyDynamicRoutesConfigurationService
    {
        public string GetController(string route)
        {
            return route switch
            {
                "WeatherForecast2" => "WeatherForecast",
                "api/WeatherForecast" => "WeatherForecast",
                "api/proxy" => "Proxy",

                _ => route
            };
        }
    }
}
