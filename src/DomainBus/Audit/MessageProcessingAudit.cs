using System;
using System.Collections.Generic;

namespace DomainBus.Audit
{
    public class MessageProcessingAudit:IAuditMessageId
    {
        public MessageProcessingAudit(Guid messageId)
        {
            MessageId = messageId;
        }
        public Guid MessageId { get; private set; }
        public DateTimeOffset StartedProcessingAt { get; set; }=DateTimeOffset.Now;

        public List<MessageProcessingHandlingAudit> Handlers { get; }=new List<MessageProcessingHandlingAudit>();

        public DateTimeOffset? CompletedAt { get; set; }

        public bool ThrewConfigurationError { get; set; }

        public bool WasSuccessful => Handlers.TrueForAll(d => d.WasSuccessful);

    }
}