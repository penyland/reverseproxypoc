using System;

namespace ReverseProxyPOC.Proxy.Models
{
    public class EventGridEvent : EventGridEvent<object>
    {
    }

    public class EventGridEvent<T>
    {
        public string Id { get; set; }

        public string Topic { get; set; }

        public string Subject { get; set; }

        public T Data { get; set; }

        public string EventType { get; set; }

        public DateTime EventTime { get; set; }

        public string DataVersion { get; set; }

        public string MetadataVersion { get; set; }
    }
}
