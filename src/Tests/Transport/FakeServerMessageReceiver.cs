using System;
using System.Collections.Generic;
using DomainBus.Configuration;
using DomainBus.Transport;


namespace Tests.Transport
{
    public class FakeServerMessageReceiver:AServerMessagesReceiver
    {
        public List<EnvelopeToClient> Envelopes { get; } = new List<EnvelopeToClient>();
        public FakeServerMessageReceiver()
        {
            
        }

        public void Add()
        {
            Envelopes.Add(new EnvelopeToClient() { To = EndpointId.TestValue, Id = Guid.NewGuid(), Messages = new[] { new MyCommand(), } });
        }
        protected override EnvelopeToClient[] GetMessages()
        {
            Stop();
            return Envelopes.ToArray();
        }
        public EnvelopeToClient Handled { get; private set; }
        protected override void MarkAsHandled(EnvelopeToClient env)
        {
            if (Handled != null) throw new Exception("Already handled");
            Handled = env;
        }
    }
}