using System;

namespace DomainBus.DomainEvents
{
    public abstract class DomainEvent : AbstractEvent
    {
        public Guid EntityId { get; set; }

        public int AggregateVersion { get; set; }

        protected DomainEvent()
        {
            
        }

        protected DomainEvent(Guid entityId)
        {
            EntityId = entityId;
        }

    }
}