using System;
using System.Threading.Tasks;
using DomainBus.Abstractions;
using DomainBus.Configuration;
using DomainBus.Dispatcher.Server;

namespace DomainBus.Transport
{
    public class ProcessingStorageTransporter : IDeliverToEndpoint
    {
        private readonly Func<IAddMessageToProcessorStorage> _factory;
        private readonly string _processorName;

        public ProcessingStorageTransporter(Func<IAddMessageToProcessorStorage> factory,EndpointId processorName)
        {
            _factory = factory;
            _processorName = processorName;
        }
        
        public async Task Send(EnvelopeToClient envelope)
        {
            var store = _factory();
            await store.Add(_processorName, envelope.Messages).ConfigureAwait(false);
            
        }
    }
}