using System;

namespace DomainBus.DomainEvents
{
    public static class Extensions
    {
        /// <summary>
        /// Stores the events using the specified action then publish them.
        /// If persisting throws a <see cref="DuplicateOperationException"/> it will use those events for publishing
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="persistAction">Should throw <see cref="DuplicateOperationException"/> if a commit is duplicate</param>
        public static void PublishAfterStoring(this IDispatchMessages bus,Func<IEvent[]> persistAction)
        {
            persistAction.MustNotBeNull();
            IEvent[] events = Array.Empty<IEvent>();
           try
           {
                events=persistAction();
           }
            catch (DuplicateOperationException ex)
            {
                events = ex.Events;
            }
           
            bus.Publish(events);            
        }

        /// <summary>
        /// Publishes and clears events
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="bus"></param>
        public static void PublishEvents(this IGenerateEvents entity,IDispatchMessages bus)
        {
            entity.MustNotBeNull();
             bus.Publish(entity.GetGeneratedEvents());
            entity.ClearEvents();
        }
    }

    
}