namespace DomainBus.DomainEvents
{
    /// <summary>
    /// Use it in command handlers to store/publish events with idempotency support
    /// </summary>
    public interface IStoreAndPublishEvents
    {
        /// <summary>
        /// Handles idempotency automatically
        /// </summary>
        /// <param name="events"></param>
        void StoreAndPublish(params DomainEvent[] events);
    }
}