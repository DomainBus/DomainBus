using DomainBus.Processing;

namespace DomainBus
{
    
    public abstract class IFindSagaState<T> where T : ISagaState
    {
        /// <summary>
        /// Uses the event to retrieve the saga state. Saga starter events don't need this.
        /// If the saga is completed the return value should be null.
        /// </summary>
        /// <exception cref="BusStorageException"></exception>
        /// <typeparam name="E"></typeparam>
        public interface Using<E> where E : IEvent
        {
            T GetSagaData(E evnt);
        }
    }
}