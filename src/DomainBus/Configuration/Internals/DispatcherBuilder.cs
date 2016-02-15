using System;
using DomainBus.Audit;
using DomainBus.Dispatcher.Client;
using DomainBus.Transport;

namespace DomainBus.Configuration.Internals
{
    public class DispatcherBuilder:IConfigureDispatcher
    {
    
      public ITalkToServer Communicator { get; private set; }


        public IConfigureDispatcher TalkUsing(ITalkToServer communicator)
        {
            communicator.MustNotBeNull();
            Communicator = communicator;
            return this;
        }

        public void Verify()
        {
            Communicator.MustNotBeNull();
            Receiver.MustNotBeNull();
        }

        public IReceiveServerMessages Receiver { get; private set; }
        public IConfigureDispatcher ReceiveMessagesUsing(IReceiveServerMessages rec)
        {
            rec.MustNotBeNull();
            Receiver = rec;
            return this;
        }

        public DispatcherClient BuildClient(string host,IDeliveryErrorsQueue errors,BusAuditor auditor) 
            => new DispatcherClient(host,Communicator,errors,auditor);
    }
}