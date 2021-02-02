namespace ReverseProxyPOC.Proxy.Services
{
    public interface IProxyDynamicRoutesConfigurationService
    {
        string GetController(string route);
    }
}
