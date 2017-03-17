namespace DomainBus.DomainEvents
{
    internal class EventsStoreAndPublisher:IStoreAndPublishEvents
    {
        private readonly IDispatchMessages _bus;
        private readonly IAppendEventsToStore _appender;
        

        public EventsStoreAndPublisher(IDispatchMessages bus,IAppendEventsToStore appender)
        {
            _bus = bus;
            _appender = appender;
        }

        public void StoreAndPublish(params DomainEvent[] events)
        {
            if (events.Length==0) return;
            try
            {
                _appender.Append(events);
            }
            catch (DuplicateOperationException ex)
            {
                events = ex.Events;
            }

            _bus.Publish(events);
        }
    }
}