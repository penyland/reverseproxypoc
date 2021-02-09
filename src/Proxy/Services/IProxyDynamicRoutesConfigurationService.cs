namespace ReverseProxyPOC.Proxy.Services
{
    public interface IProxyDynamicRoutesConfigurationService
    {
        void Initialize();

        (string Controller, string Action) GetController(string route);
    }
}
