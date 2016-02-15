using System;

namespace DomainBus.Audit
{
    public interface IAuditMessageId
    {
        Guid MessageId { get; }
    }
}