using System;
using DomainBus.Abstractions;

namespace DomainBus.Transport
{
    public class EnvelopeFromClient
    {
        
        public IMessage[] Messages { get; set; } = Array.Empty<IMessage>();
        /// <summary>
        /// Origin host
        /// </summary>
        public string From { get; set; }
        
    }
}