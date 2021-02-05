namespace ReverseProxyPOC.Proxy.Services
{
    public class ProxyDynamicRoutesConfigurationService : IProxyDynamicRoutesConfigurationService
    {
        public (string Controller, string Action) GetController(string route)
        {
            return route switch
            {
                "WeatherForecast" => ("WeatherForecast", "GetForecasts"),
                "WeatherForecast/2" => ("WeatherForecast", "GetForecast"),
                "api/WeatherForecast" => ("WeatherForecast", "GetForecast"),
                "api/proxy" => ("Proxy", "Get"),
                "Todo" => ("Todo", "GetTodoItems"),

                _ => (route, string.Empty)
            };
        }
    }
}
