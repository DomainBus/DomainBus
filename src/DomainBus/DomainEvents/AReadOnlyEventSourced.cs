using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainBus.DomainEvents
{
    public abstract class AReadOnlyEventSourced
    {
        protected AReadOnlyEventSourced(IEnumerable<DomainEvent> events)
        {
            events.MustNotBeNull();
            events.ForEach(ApplyChange);
        }

        protected virtual void ApplyChange(DomainEvent ev)
        {
            this.AsDynamic().Apply((dynamic)ev);            
        }
    }
}