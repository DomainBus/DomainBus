using System;
using DomainBus.Abstractions;

namespace DomainBus.Processing.Internals
{
    public class MessageProcessor:IProcessMessage
    {
        private readonly IKnowMessageHandlers _handlers;

        public MessageProcessor(IKnowMessageHandlers handlers)
        {
            _handlers = handlers;
        }

        public  void Process(IMessage msg, string processor)
        {
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
    }
}