using System;
using System.Collections.Generic;
using CavemanTools.Testing;
using DomainBus.Transport;


namespace Tests.Transport
{
    public class FakeClientMEssagesReceiver:AClientMessagesReceiver
    {
        public List<EnvelopeFromClient> Envelopes { get; }=new List<EnvelopeFromClient>();
        public FakeClientMEssagesReceiver():base(new StubTimer())
        {
            
        }

        public void Add()
        {
            Envelopes.Add(new EnvelopeFromClient(){From = "remote",Id = Guid.NewGuid(),Messages = new []{new MyCommand(), }});
        }

    
        protected override EnvelopeFromClient[] GetMessages()
        {
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