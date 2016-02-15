using System;

namespace DomainBus.Dispatcher
{
    /// <summary>
    /// Used as a singleton, must be thread safe
    /// </summary>
    public interface IStoreReservedMessagesIds
    {
        Guid[] Get(ReservedIdsSource input);
        void Add(ReservedIdsSource id,Guid[] ids);
      
    }
}