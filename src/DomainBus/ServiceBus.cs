using System;
using System.Linq;
using CavemanTools.Infrastructure;
using DomainBus.Configuration;
using DomainBus.Configuration.Internals;
using DomainBus.Dispatcher;
using DomainBus.Dispatcher.Client;
using DomainBus.Transport;

namespace DomainBus
{
    public class ServiceBus:IDomainBus
    {
        private readonly DispatcherClient _client;
        private readonly ConfigureHost _host;
        private readonly IReceiveServerMessages _receiver;

        private EndpointConfig[] _endpoints;


        public const string MemoryProcessor = "memory";

        public static IBuildBusWithContainer ConfigureWith(IBuildBusWithContainer wrapper)
        {
            return wrapper;
        }

        internal ServiceBus(IContainerScope container,DispatcherClient client, ConfigureHost host,IReceiveServerMessages receiver)
        {
            Container = container;
            _client = client;
            _host = host;
            _receiver = receiver;
            _endpoints = host.Endpoints;
        }

        public void Dispose()
        {
            if (_endpoints != null)
            {
                _endpoints.ForEach(e => e.Processor.Dispose());
                _endpoints = null;
            }
            
        }

        public void StartListeningForMessages()
        {
            _receiver.StartReceiving(_client);
        }

        public IDispatchMessages GetDispatcher() => new MessageDispatcher(_client.Dispatch,_host.GetStorage<IStoreReservedMessagesIds>());
        public IDispatchReceivedMessages GetReceiver() => _client;


        /// <exception cref="DomainBusConfigurationException"></exception>
        public IManageProcessingService GetProcessingQueue(string name)
        {
            var ep= _endpoints.FirstOrDefault(d => d.Id.Processor == name);
            if (ep == null)
            {
                throw new DomainBusConfigurationException("There is no processor named {0}".ToFormat(name));
            }

            return ep.Processor;
        }

        public void StartProcessors() => _endpoints.ForEach(d=>d.Processor.Start());

        public IContainerScope Container { get; }
    }
}