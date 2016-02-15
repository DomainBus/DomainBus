using System;

namespace DomainBus.Configuration
{
    public interface IConfigureProcessingService
    {
      
        /// <summary>
        /// Gets/sets how often the storage is checked for pending messages. Default is 1 minute
        /// </summary>
        TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// Get/sets how many messages should be loaded from the storage. Default is 15
        /// </summary>
        int BufferSize { get; set; }

        /// <summary>
        /// Enables or disables message polling. If disabled, only messages added in-process will be processed.
        /// Should be used only when you don't care about storage.
        /// </summary>
        bool PollingEnabled { get; set; }

        
    }
}