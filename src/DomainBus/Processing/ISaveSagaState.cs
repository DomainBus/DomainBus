namespace DomainBus.Processing
{
    public interface ISaveSagaState<T> where T : ISagaState
    {
        /// <summary>
        /// Persist saga data. Throws if an existing saga is newer than the saved saga.
        /// </summary>
        /// <exception cref="SagaConcurrencyException"></exception>
        /// <exception cref="SagaExistsException"></exception>
        /// <exception cref="BusStorageException"></exception>
        /// <param name="data"></param>
        /// <param name="isNew">True if saga just started</param>
        void Save(T data, bool isNew);
    }
}