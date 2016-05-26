using System;
using System.Collections.Generic;
using System.Linq;
using CavemanTools.Model;

namespace DomainBus.DomainEvents
{
    public abstract class EventSourcedEntity:AnEntityWithOperationId
    {

        public int Version { get; private set; } = -1;

        private List<DomainEvent> _history=new List<DomainEvent>();


        protected List<DomainEvent> Events = new List<DomainEvent>();


        protected void Apply<T>(T evnt, Action<T> action) where T : DomainEvent
        {
            action(evnt);
            AddEvent(evnt);
        }

        /// <summary>
        /// Sets operation id, entity id and agg version then adds event to generated events list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="evnt"></param>
        protected void AddEvent<T>(T evnt) where T : DomainEvent
        {
            if (_operationId == null) throw new NullReferenceException("Operation id is not set");
            evnt.OperationId = _operationId.Value;
            evnt.EntityId = EntityId;
            evnt.AggregateVersion = Version;
            Events.Add(evnt);
        }


        public DomainEvent[] GetGeneratedEvents() => Events.ToArray();

        public void ClearEvents() => Events.Clear();
        public Guid EntityId { get; protected set; }

        protected EventSourcedEntity(IEnumerable<DomainEvent> events):this()
        {
            events.MustNotBeEmpty();
            Restore(events);
          
        }

        protected EventSourcedEntity()
        {
           
        }

        protected void Restore(IEnumerable<DomainEvent> events)
        {
            _history.AddRange(events);
            _history.ForEach(ev => ApplyChange(ev, false));           
        }


        /// <summary>
        /// Sets the operation id to ensure that an operation (calling one or more methods) can't be  repeated. 
        /// Used to ensure idempotency
        /// <exception cref="DuplicateOperationException"></exception>
        /// </summary>
        /// <param name="id"></param>
        public override void SetOperationId(Guid id)
        {
            if (_history.Any(e=>e.OperationId==id))
            {
                throw new DuplicateOperationException(id,GetEventsForOperation(id).ToArray());
            }
           base.SetOperationId(id);
            Version++;
        }

        /// <summary>
        /// All changes are added to history
        /// </summary>
        public void MarkAsCommited()
        {
            _history.AddRange(GetGeneratedEvents());
            ClearEvents();
        }
       
        public bool HasChanges() => Events.Count > 0;


        /// <summary>
        /// Used to retrieve already comitted events when <see cref="DuplicateOperationException"/> is thrown.
        /// That usually happens if a command wasn't completed sucessfully and the events weren't published.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<DomainEvent> GetEventsForOperation(Guid id) => _history.Where(e => e.OperationId == id);

        /// <summary>
        /// Also sets the event's operation id, entity id and agg version
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="isNew"></param>
        protected virtual void ApplyChange(DomainEvent ev, bool isNew = true)
        {
            this.AsDynamic().Apply((dynamic)ev);
            
            if (isNew) AddEvent(ev);
            else Version = ev.AggregateVersion;
        }
     
    }
}