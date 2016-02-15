using System;
using DomainBus.Abstractions;
using DomainBus.Configuration;

namespace DomainBus.Transport
{
    public class EnvelopeTo
    {
        
        public IMessage[] Messages { get; set; } = Array.Empty<IMessage>();
        /// <summary>
        /// Destination
        /// </summary>
        public EndpointId To { get; set; }
        
    }
}