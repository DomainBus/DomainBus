namespace DomainBus.DomainEvents
{
    /// <summary>
    /// Implementation should save events to the underlying event store.
    ///  Used by <see cref="IStoreAndPublishEvents"/> implementation.
    /// </summary>
    public interface IAppendEventsToStore
    {
        /// <summary>
        /// Implementor should throw if the event store detects a duplicate commit. 
        /// </summary>
        /// <param name="events"></param>
        /// <exception cref="DuplicateOperationException">Exception should contain the committed events associated with the operation id</exception>
        void Append(params DomainEvent[] events);
    }
}