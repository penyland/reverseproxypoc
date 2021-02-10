using ReverseProxyPOC.Proxy.Proxy;

namespace ReverseProxyPOC.Proxy.Services
{
    public interface IProxyDynamicRoutesConfigurationService
    {
        void Initialize();

        (string Controller, string Action) GetController(string route);

        EndpointInfo ResolveDynamicEndpoint(string controller, string method, string action);
    }
}
