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

    public static class DomainBusEx
    {
        /// <summary>
        /// Creates an instance (that should be registered as singleton in a DI Container) of a mediator 
        /// which waits for a command handler result, executed by the domain bus in the same process.
        /// The command handler will need to take <see cref="ICommandResultMediator"/> as a dependency to communicate the result
        /// in the form of <see cref="CommandResult"/>. 
        /// This feature is useless in distributed apps.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="mediator"></param>
        /// <returns></returns>
        public static IRequestCommandResult GetCommandResultMediator(this IDomainBus bus,ICommandResultMediator mediator)
            =>new BusWithCommandResultMediator(bus,mediator);
    }
}