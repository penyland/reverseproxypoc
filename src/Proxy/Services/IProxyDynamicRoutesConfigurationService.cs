namespace ReverseProxyPOC.Proxy.Services
{
    public interface IProxyDynamicRoutesConfigurationService
    {
        (string Controller, string Action) GetController(string route);
    }
}
