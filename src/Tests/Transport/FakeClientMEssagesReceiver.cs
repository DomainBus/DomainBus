using System;
using System.Collections.Generic;
using DomainBus.Transport;


namespace Tests.Transport
{
    public class FakeClientMEssagesReceiver:AClientMessagesReceiver
    {
        public List<EnvelopeFromClient> Envelopes { get; }=new List<EnvelopeFromClient>();
        public FakeClientMEssagesReceiver()
        {
            PollingInterval = 50.ToMiliseconds();
        }

        public void Add()
        {
            Envelopes.Add(new EnvelopeFromClient(){From = "remote",Id = Guid.NewGuid(),Messages = new []{new MyCommand(), }});
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