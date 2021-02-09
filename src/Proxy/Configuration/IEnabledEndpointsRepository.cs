namespace ReverseProxyPOC.Proxy.Configuration
{
    internal interface IEnabledEndpointsRepository
    {
        bool IsEndpointEnabled(string endpointName);

        bool ShouldProxy();
    }

    interface IProxyApiEndpointInfo
    {

    }

    //public class ApiHelpEndpointViewModel
    //{
    //    public string Endpoint { get; set; }
    //    public string Controller { get; set; }
    //    public string Action { get; set; }
    //    public string DisplayableName { get; set; }
    //    public string Description { get; set; }
    //    public string EndpointRoute => $"/api/{Endpoint}";
    //    public PropertyInfo[] Properties { get; set; }
    //    public List<IList<CustomAttributeTypedArgument>> PropertyDescription { get; set; }
    //}
}
