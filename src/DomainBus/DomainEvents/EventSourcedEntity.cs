using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainBus.DomainEvents
{
    public abstract class EventSourcedEntity:AnEntityGeneratingEvents
    {
   
        private List<IEvent> _history=new List<IEvent>();


       protected EventSourcedEntity(IEnumerable<IEvent> events):this()
        {
            events.MustNotBeEmpty();
            _history.AddRange(events);
            Restore();
        }

        protected EventSourcedEntity()
        {
           
        }

        protected void Restore() => _history.ForEach(ev => ApplyChange(ev, false));

   
      
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
        public IEnumerable<IEvent> GetEventsForOperation(Guid id) => _history.Where(e => e.OperationId == id);

        /// <summary>
        /// Also sets the event's operation id
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="isNew"></param>
        protected virtual void ApplyChange(IEvent ev, bool isNew = true)
        {
            this.AsDynamic().Apply((dynamic)ev);
            if (!isNew) return;

            AddEvent(ev);
        }
     
    }
}