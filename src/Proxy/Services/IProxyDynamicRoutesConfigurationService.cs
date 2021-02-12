namespace ReverseProxyPOC.Proxy.Services
{
    public interface IProxyDynamicRoutesConfigurationService
    {
        bool IsEnabled(string routeName);
    }
}
