using System;

namespace DomainBus.Transport
{
    public class CouldntSendMessagesException:Exception
    {
        public EnvelopeFromClient From { get; set; }
        public EnvelopeToClient To { get; set; }

        public CouldntSendMessagesException(EnvelopeToClient to,string message,Exception inner):base(message,inner)
        {
            To = to;        
        }

        public CouldntSendMessagesException(EnvelopeFromClient from, string message, Exception inner) : base(message, inner)
        {
            From = @from;
        }
    }
}