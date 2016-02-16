using System;
using System.Collections.Generic;
using DomainBus.Transport;
using Ploeh.AutoFixture;

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
            Envelopes.Add(Setup.Fixture.Create<EnvelopeToClient>());
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