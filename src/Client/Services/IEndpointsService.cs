using ReverseProxyPOC.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Client.Services
{
    public interface IEndpointsService
    {
        Task<IEnumerable<EndpointInfo>> GetEndpointsAsync();
    }
}