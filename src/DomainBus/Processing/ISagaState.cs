using System;

namespace DomainBus.Processing
{
    public interface ISagaState
    {
       Guid Id { get;  }
        bool IsCompleted { get; set; }

        /// <summary>
        /// Required to handle concurrency. Only repositories should modify it
        /// </summary>
        long AutoTimestamp { get; set; }
    }
}