namespace ReverseProxyPOC.Proxy.Configuration
{
    internal interface IEnabledEndpointsRepository
    {
        bool IsEndpointEnabled(string endpointName);
    }
}
