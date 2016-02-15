using System.Collections.Generic;
using DomainBus.Configuration;

namespace DomainBus.Dispatcher.Server
{
    public class TransportersHub
    {
      
        Dictionary<EndpointId,IDeliverToEndpoint>  _transporters=new Dictionary<EndpointId, IDeliverToEndpoint>();

        public void Add(EndpointId id, IDeliverToEndpoint instance) => _transporters.Add(id, instance);

        public IDeliverToEndpoint GetTransporter(EndpointId endpoint)
            => _transporters.GetValueOrDefault(endpoint);
    }
}