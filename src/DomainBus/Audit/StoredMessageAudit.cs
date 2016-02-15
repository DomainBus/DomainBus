using System;

namespace DomainBus.Audit
{
    public class StoredMessageAudit : IAuditMessageId
    {
        public Guid MessageId { get; set; }
        public DateTimeOffset AddedAt { get; set; }=DateTimeOffset.Now;

        public string Processor { get; set; }

    }
}