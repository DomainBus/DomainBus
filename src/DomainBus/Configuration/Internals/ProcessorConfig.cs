using System;
using System.Collections.Generic;
using System.Linq;
using CavemanTools;
using CavemanTools.Infrastructure;
using DomainBus.Audit;
using DomainBus.Processing;
using DomainBus.Processing.Internals;

namespace DomainBus.Configuration.Internals
{
    internal class ProcessorConfig : IConfigureProcessors
    {
        Dictionary<IConfigureHostedEndpoint,Action<IConfigureProcessingService>> _points=new Dictionary<IConfigureHostedEndpoint, Action<IConfigureProcessingService>>();
        
        #region Implementation of IConfigureProcessors

        IConfigureProcessors IConfigureProcessors.ForEndpoint(IConfigureHostedEndpoint endpoint, Action<IConfigureProcessingService> cfg)
        {
            endpoint.MustNotBeNull();
            _points.Add(endpoint,cfg??Empty.ActionOf<IConfigureProcessingService>());
            return this;
        }

        #endregion

        public void Verify()
        {
            _points.MustNotBeEmpty();
        }

        internal EndpointConfig[] Build(ConfigureHost host, IContainerScope container, BusAuditor auditor) 
            => _points.Select(ec =>
        {
            var ep = ec.Key;
            var nexus = new MessageHandlersNexus(container,auditor,host);
            nexus.Add(host.Handlers.Where(ep.CanHandle).ToArray());
            var relayer=new RelayLocalEvents(host.Relayer);
            var processor = new ProcessingService(host.GetStorage<IStoreUnhandledMessages>(), () => new MessageProcessor(nexus,relayer), auditor,host.GetStorage<IFailedMessagesQueue>());
            ec.Value(processor);

            var config = new EndpointConfig(processor);
            config.Id = new EndpointId(ep.Name, host.HostName);
            config.HandledMessagesTypes = nexus.GetMessageTypes().ToArray();
            processor.Name = config.Id;
            return config;
        }).ToArray();
    }
}