using System.Collections.Generic;

namespace DomainBus.Dispatcher.Client
{
    public interface IDispatcherClient
    {
       void SubscribeToServer(IEnumerable<IEndpointConfiguration> configs);
    }
}