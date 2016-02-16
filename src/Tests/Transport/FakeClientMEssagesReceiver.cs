using System;
using System.Collections.Generic;
using DomainBus.Transport;
using Ploeh.AutoFixture;

namespace Tests.Transport
{
    public class FakeClientMEssagesReceiver:AClientMessagesReceiver
    {
        public List<EnvelopeFromClient> Envelopes { get; }=new List<EnvelopeFromClient>();
        public FakeClientMEssagesReceiver()
        {
            }

        public void Add()
        {
            Envelopes.Add(Setup.Fixture.Create<EnvelopeFromClient>());
        }

    
        protected override EnvelopeFromClient[] GetMessages()
        {
            Stop();
            return Envelopes.ToArray();
        }

        public EnvelopeFromClient Handled { get; private set; }

        protected override void MarkAsHandled(EnvelopeFromClient envs)
        {
            if (Handled!=null) throw new Exception("Already handled");
            Handled = envs;
        }
    }
}