using System;
using DomainBus.Configuration;

namespace DomainBus.Dispatcher
{
    public class EndpointNotFoundException : Exception
    {
        public EndpointId Endpoint { get; set; }

        public EndpointNotFoundException(EndpointId endpoint, string host) : base($"'{endpoint}' not found on host '{host}'")
        {
            Endpoint = endpoint;
        }
    }
}