using System;
using System.Collections.Generic;
using DomainBus.Audit;
using DomainBus.Dispatcher;
using DomainBus.Processing;
using DomainBus.Transport;

namespace DomainBus.Configuration
{
    /// <summary>
    /// Must be configured: container integration, handler types and processors.
    /// 
    /// Optional: (recommended)processing storage (with reserve id storage), saga storage, audit storage, error queues
    /// 
    /// </summary>
    public interface IConfigureHost
    {
        /// <summary>
        /// Default is null storage, no durability
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        IConfigureHost WithProcessingStorage(IStoreUnhandledMessages store);
        /// <summary>
        /// Default is null storage, no durability
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        IConfigureHost WithReserveIdStorage(IStoreReservedMessagesIds store);

        /// <summary>
        ///  Default is null storage, no durability
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        IConfigureHost SendFailedMessagesTo(IFailedMessagesQueue queue);
        /// <summary>
        /// Default is null storage, no durability
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        IConfigureHost SendFailedDeliveriesTo(IDeliveryErrorsQueue queue);
        /// <summary>
        /// Default is null storage
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        IConfigureHost PersistAuditsWith(IStoreAudits store);
        /// <summary>
        /// Name for the app hosting the service bus as a client
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IConfigureHost HostnameIs(string name);
        IConfigureHost ConfigureSagas(Action<IConfigureSagas> cfg);

        /// <summary>
        /// First step of integration with a DI Container
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <returns></returns>
        IConfigureHost RegisterTypesInContainer(IRegisterBusTypesInContainer containerBuilder);

        /// <summary>
        /// DomainBus will automatically identify and use message handlers and saga states from the specified types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        IConfigureHost AutoConfigureFrom(IEnumerable<Type> types);
        /// <summary>
        /// Configuration for the hosted message processors
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        IConfigureHost ConfigureProcessors(Action<IConfigureProcessors> cfg);

        /// <summary>
        /// Relays all events decorated . Available only for local host.
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        IConfigureHost RelayEventsLocally(Action<IConfigureLocalRelay> cfg);

    }

    public interface IConfigureLocalRelay
    {
        IConfigureLocalRelay Send(Action<IEvent> action);
    }

    public interface IConfigureSagas
    {
        /// <summary>
        /// By default we can use a generic storage which works for any saga state
        /// </summary>
        /// <returns></returns>
        IConfigureSagas EnableUserDefinedRepositories();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        IConfigureSagas WithSagaStorage(IStoreSagaState store);
 
    }

    public interface IConfigureProcessors
    {
        IConfigureProcessors ForEndpoint(IConfigureHostedEndpoint endpoint,Action<IConfigureProcessingService> cfg=null);
    }
} 