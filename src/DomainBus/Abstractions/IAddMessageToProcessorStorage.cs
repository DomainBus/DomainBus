using System.Collections.Generic;
using System.Threading.Tasks;
using DomainBus.Processing;

namespace DomainBus.Abstractions
{
    public interface IAddMessageToProcessorStorage
    {
        /// <summary>
        /// Duplicate messages should be ignored.
        /// Duplicates are: messages with the same id, events of the same type with the same sourceid
        /// </summary>
        /// <exception cref="BusStorageException"></exception>
        /// <param name="queueId"></param>
        /// <param name="items"></param>
        Task Add(string queueId, IEnumerable<IMessage> items);
    }
}