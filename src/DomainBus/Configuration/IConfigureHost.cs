using System;
using System.Collections.Generic;
using DomainBus.Audit;
using DomainBus.Processing;
using DomainBus.Transport;

namespace DomainBus.Configuration
{
    public interface IConfigureHost
    {
        IConfigureHost WithProcessingStorage(IStoreUnhandledMessages store);
        IConfigureHost WithReserveIdStorage(IStoreUnhandledMessages store);
        IConfigureHost SendFailedMessagesTo(IFailedMessagesQueue queue);
        IConfigureHost SendFailedDeliveriesTo(IDeliveryErrorsQueue queue);
        IConfigureHost PersistAuditsWith(IStoreAudits store);
        IConfigureHost HostnameIs(string name);
        IConfigureHost ConfigureSagas(Action<IConfigureSagas> cfg);
       

        /// <summary>
        /// DomainBus will automatically identify and use message handlers and saga states from the specified types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        IConfigureHost AutoConfigureFrom(IEnumerable<Type> types);

        IConfigureHost ConfigureProcessors(Action<IConfigureProcessors> cfg);

    }

    public interface IConfigureSagas
    {
       IConfigureSagas EnableUserDefinedRepositories();
       IConfigureSagas WithSagaStorage(IStoreSagaState store);
 
    }

    public interface IConfigureProcessors
    {
        IConfigureProcessors ForEndpoint(IConfigureHostedEndpoint endpoint,Action<IConfigureProcessingService> cfg=null);
    }
} 