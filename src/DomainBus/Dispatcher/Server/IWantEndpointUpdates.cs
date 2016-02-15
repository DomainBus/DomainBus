using System.Collections.Generic;
using DomainBus.Dispatcher.Client;

namespace DomainBus.Dispatcher.Server
{
    public interface IWantEndpointUpdates
    {
        void ReceiveConfigurations(IEnumerable<EndpointMessagesConfig> update);
    }
}