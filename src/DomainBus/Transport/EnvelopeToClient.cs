using System;
using DomainBus.Abstractions;
using DomainBus.Configuration;

namespace DomainBus.Transport
{
    public class EnvelopeToClient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IMessage[] Messages { get; set; } = Array.Empty<IMessage>();
        /// <summary>
        /// Destination
        /// </summary>
        public EndpointId To { get; set; }
        
    }
}