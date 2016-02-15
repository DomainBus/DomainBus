using System;
using DomainBus.Abstractions;

namespace DomainBus.Processing
{
    public class HandledMessageException : Exception
    {
        public Type HandlerType { get; set; }

      public HandledMessageException(Type handlerType,IMessage msg,Exception inner):base("Handling message {0} with handler {1} threw exception".ToFormat(msg.Id,handlerType),inner)
        {
            HandlerType = handlerType;
        }
    }
}