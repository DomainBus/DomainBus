using System;
using System.Collections.Generic;
using DomainBus.Abstractions;

namespace DomainBus.Processing
{
    /// <summary>
    /// Implementation must be thread safe, it will be used as a singleton
    /// </summary>
    public interface IStoreUnhandledMessages : IAddMessageToProcessorStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="BusStorageException"></exception>
        IEnumerable<IMessage> GetMessages(string queueId, int take);


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="BusStorageException"></exception>
        /// <param name="queue"></param>
        /// <param name="id"></param>
        void MarkMessageHandled(string queue, Guid id);

       
        
        
    }
}