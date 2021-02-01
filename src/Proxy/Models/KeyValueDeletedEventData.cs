namespace ReverseProxyPOC.Proxy.Models
{
    public class KeyValueDeletedEventData
    {
        public string Key { get; set; }

        public string Label { get; set; }

        public string Etag { get; set; }
    }
}
