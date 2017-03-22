using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        private DispatcherClient _client;
        private ConfigureHost _host;
        private IReceiveServerMessages _receiver;

        private EndpointConfig[] _endpoints;

        /// <summary>
        /// Name of the single processor when bus is in memory mode
        /// </summary>
        public const string MemoryProcessor = "memory";

        /// <summary>
        /// Name of processor to handle commands. For convenience only. Processor configuration needs to be explicit inside configuration
        /// </summary>
        public const string CommandsProcessor = "commands";
        /// <summary>
        /// Name of processor to handle events. For convenience only. Processor configuration needs to be explicit inside configuration
        /// </summary>
        public const string EventsProcessor = "events";

        private static BusConfigurator configurator;

       

        /// <summary>
        /// Bus will be configured for 1 process, non-distributed app
        /// </summary>
        /// <param name="cfgHost"></param>
        public static IBuildBus ConfigureForMonolith(Action<IConfigureHost> cfgHost)
        {
            Configure(c => cfgHost(c.LocalHost()),s=>s.LocalDispatcher());
            return configurator;
        }

        /// <summary>
        /// Should be used for testing/development/debugging. NOT recommended for production.
        /// </summary>
        /// <param name="containerCfg"></param>
        /// <param name="asms">Assemblies containing message handlers</param>
        public static IBuildBus ConfigureAsMemoryBus(IRegisterBusTypesInContainer containerCfg, params Assembly[] asms)
        {
            return ConfigureAsMemoryBus(containerCfg, asms.SelectMany(a => a.GetExportedTypes().Where(BusBuilderExtensions.IsMessageHandler)).ToArray());            
        }

        /// <summary>
        /// Should be used for testing/development/debugging. NOT recommended for production.
        /// 
        /// </summary>
        /// <param name="containerCfg">Container builder</param>
        /// <param name="handlerTypes"></param>
        public static IBuildBus ConfigureAsMemoryBus(IRegisterBusTypesInContainer containerCfg,params Type[] handlerTypes)
        {                
              return ConfigureForMonolith(cfg =>
              {
                  cfg
                  .RegisterTypesInContainer(containerCfg)
                  .WithInMemoryAudits()
                  .AutoConfigureFrom(handlerTypes)
                  .ConfigureProcessors(procs => procs.Add(MemoryProcessor, endpoint => endpoint.HandlesEverything()));
              });
        }

        /// <summary>
        /// Configure local host and server communication. Use it when you plan to use DomainBus in a distributed app
        /// </summary>
        /// <param name="cfgHost"></param>
        /// <param name="cfgServer"></param>
        public static IBuildBus Configure(Action<IConfigureHost> cfgHost, Action<IConfigureDispatcher> cfgServer)
        {
            configurator=new BusConfigurator(cfgHost,cfgServer);
            return configurator;
        }

         /// <summary>
        /// Builds the bus. Container already has registrations for <see cref="IDomainBus"/> as singleton
        /// and <see cref="IDispatchMessages"/>
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IDomainBus Build(IContainerScope container)
        {
            configurator.MustNotBeNull(ex:new DomainBusConfigurationException("DomainBus is not configured. Invoke `ServiceBus.Configure()` first."));
            return configurator.Build(container);
        }


        internal ServiceBus()
        {
            
        }

        internal void Init(DispatcherClient client, ConfigureHost host,IReceiveServerMessages receiver)
        {        
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

        public void StartListeningForMessages() => _receiver.StartReceiving(_client);

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
        
    }

    public interface IBuildBus
    {
        IDomainBus Build(IContainerScope container);
    }
   
}