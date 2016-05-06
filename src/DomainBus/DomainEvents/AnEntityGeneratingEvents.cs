using System;
using System.Collections.Generic;
using CavemanTools.Model;

namespace DomainBus.DomainEvents
{
    public abstract class AnEntityGeneratingEvents : AnEntityWithOperationId,IEntityGeneratingEvents,IEntityId
    {
        protected List<IEvent>  Events=new List<IEvent>();


        protected void Apply<T>(T evnt,Action<T> action) where T : IEvent
        {
            action(evnt);
            AddEvent(evnt);
        }

        protected void AddEvent<T>(T evnt) where T : IEvent
        {
            if (_operationId == null) throw new NullReferenceException("Operation id is not set");
            evnt.OperationId = _operationId.Value;
            Events.Add(evnt);
        }


        public IEvent[] GetGeneratedEvents() => Events.ToArray();

        public void ClearEvents() => Events.Clear();
        public Guid EntityId { get; protected set; }
    }
}