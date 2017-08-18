using System;
using DomainBus.Abstractions;

namespace DomainBus.Processing.Internals
{
    public class MessageProcessor:IProcessMessage
    {
        private readonly IKnowMessageHandlers _handlers;
        private readonly IRelayLocalEvents _relayer;

        public MessageProcessor(IKnowMessageHandlers handlers,IRelayLocalEvents relayer)
        {
            _handlers = handlers;
            _relayer = relayer;
        }

        public  void Process(IMessage msg, string processor)
        {
            Relay(msg as IEvent);
            var invoker = _handlers.GetHandlerInvoker(msg.GetType());
            if (invoker == null) 
            {
                if (msg is ICommand) 
                {
                    throw new MissingHandlerException(msg);                
                }
                if (msg is IEvent)//an event can have 0 subscribers
                {
                    return;
                }
                throw new NotSupportedException("Only commands and events are supported");
            }

            invoker.Handle(msg,processor);
           
        }

        private void Relay(IEvent @event)
        {
            if (@event==null) return;
            _relayer.Queue(@event);
        }
    }
}