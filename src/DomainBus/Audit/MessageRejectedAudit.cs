using System;

namespace DomainBus.Audit
{
    public class MessageRejectedAudit:IAuditMessageId
    {
        public Guid MessageId { get; private set; }
        public string Destination { get; set; }
      
        public DateTimeOffset ReceivedAt { get; }=DateTimeOffset.Now;

        public MessageRejectedAudit()
        {
            
        }       

        public MessageRejectedAudit(Guid messageId,string destination)
        {
            MessageId = messageId;
            Destination = destination;
        }
    }
}