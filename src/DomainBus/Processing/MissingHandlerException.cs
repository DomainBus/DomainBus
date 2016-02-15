using System;
using DomainBus.Abstractions;

namespace DomainBus.Processing
{
    public class MissingHandlerException : Exception
    {
        public IMessage Msg { get; set; }

        public MissingHandlerException(IMessage msg) : base("There's no handler for message {0}".ToFormat(msg))
        {
            Msg = msg;
        }
    }
}