using System;

namespace DomainBus.Processing
{
    /// <summary>
    /// Used as a singleton.
    /// If an user defined repository is configured, it will be used insted of this.
    /// </summary>
    public interface IStoreSagaState
    {
        ///  <summary>
        /// 
        ///  </summary>
        /// <param name="correlationId">Unique saga correlation id</param>
        /// <param name="sagaStateType"></param>
        /// <exception cref="BusStorageException"></exception>
        /// <returns></returns>
        ISagaState GetSaga(string correlationId, Type sagaStateType);

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="SagaConcurrencyException"></exception>
        /// <exception cref="SagaExistsException"></exception>
        /// <exception cref="BusStorageException"></exception>
        /// <param name="data"></param>
        /// <param name="correlationId">This should be unique</param>
        /// <param name="isNew">True if the saga just started</param>
        void Save(ISagaState data, string correlationId, bool isNew);
       
    }
}