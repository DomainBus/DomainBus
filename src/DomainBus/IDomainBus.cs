using System;
using CavemanTools.Infrastructure;
using DomainBus.Configuration;
using DomainBus.Dispatcher.Client;

namespace DomainBus
{
    public interface IDomainBus:IDisposable
    {
        IDispatchMessages GetDispatcher();

        IDispatchReceivedMessages GetReceiver();

        /// <summary>
        /// Gets hosted processing queue manager
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        IManageProcessingService GetProcessingQueue(string name);
        
        void StartProcessors();

        IContainerScope Container { get; }

        void StartListeningForMessages();
    }
    
}