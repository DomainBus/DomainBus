using System;

namespace DomainBus.DomainEvents
{
    public class DuplicateOperationException : Exception
    {
        public Guid OperationId { get; set; }
        public DomainEvent[] Events { get; set; }

        public DuplicateOperationException(Guid operationId,DomainEvent[]events)
        {
            OperationId = operationId;
            Events = events;
        }
    }
}