using System;

namespace DomainBus.DomainEvents
{
    public class DuplicateOperationException : Exception
    {
        public Guid OperationId { get; set; }
        public IEvent[] Events { get; set; }

        public DuplicateOperationException(Guid operationId,IEvent[]events)
        {
            OperationId = operationId;
            Events = events;
        }
    }
}