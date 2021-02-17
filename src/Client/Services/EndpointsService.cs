using ReverseProxyPOC.Client.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Client.Services
{
    public class EndpointsService : IEndpointsService
    {
        private readonly HttpClient httpClient;

        public EndpointsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<EndpointInfo>> GetEndpointsAsync()
        {
            var endpoints = await this.httpClient.GetFromJsonAsync<IEnumerable<EndpointInfo>>("/endpoints");

            return endpoints;
        }
    }
}
