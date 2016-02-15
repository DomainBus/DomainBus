using System;

namespace DomainBus.Transport
{
    public class CouldntSendMessagesException:Exception
    {
        public EnvelopeFrom From { get; set; }
        public EnvelopeTo To { get; set; }

        public CouldntSendMessagesException(EnvelopeTo to,string message,Exception inner):base(message,inner)
        {
            To = to;        
        }

        public CouldntSendMessagesException(EnvelopeFrom from, string message, Exception inner) : base(message, inner)
        {
            From = @from;
        }
    }
}