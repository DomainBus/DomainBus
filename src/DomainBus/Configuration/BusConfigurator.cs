using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CavemanTools.Infrastructure;
using DomainBus.Audit;
using DomainBus.Configuration.Internals;
using DomainBus.Processing;
using DomainBus.Transport;

namespace DomainBus.Configuration
{
    internal class BusConfigurator:IBuildBus
    {
       
        private DispatcherBuilder _dispBuilder;
        private ConfigureHost _host;
            
        ServiceBus _bus=new ServiceBus();

        public BusConfigurator(Action<IConfigureHost> cfgHost,Action<IConfigureDispatcher> cfgServer)
        {
             CurrentHost(cfgHost);
            ServerComunication(cfgServer);
        }

        private void ServerComunication(Action<IConfigureDispatcher> cfg)
        {
            _dispBuilder = new DispatcherBuilder();
            cfg(_dispBuilder);
        
        }

        private void CurrentHost(Action<IConfigureHost> cfg)
        {
            cfg.MustNotBeNull();
            _host = new ConfigureHost();
            cfg(_host);
            RegisterTypesInContainer(_host.ContainerBuilder);
        
        }



        void RegisterTypesInContainer(IRegisterBusTypesInContainer cb)
        {
            cb.RegisterSingletonInstance(new BusAuditor(_host.GetStorage<IStoreAudits>()));
            cb.RegisterSingletonInstance(_host.GetStorage<IStoreSagaState>());

            _host.Handlers.MustNotBeNull("Handlers are null");
            _host.SagaStateTypes.MustNotBeNull("Saga state types are null");
            Func<Type, bool> isSingleton = t => t.HasCustomAttribute<SingletonHandlerAttribute>();
            cb.Register(_host.Handlers.Where(t => !isSingleton(t)).ToArray());
            cb.Register(_host.Handlers.Where(t => isSingleton(t)).ToArray(), true);

            cb.Register(_host.SagaStateTypes);
            
            cb.RegisterSingletonInstance(_bus);
            cb.RegisterInstanceFactory(_bus.GetDispatcher);
        }

        public IDomainBus Build(IContainerScope container)
        {
            _dispBuilder.MustNotBeNull("Dispatcher must be configured");
            _host.MustNotBeNull("Processors must be configured");

            _host.VerifyWeHaveAll();
            _dispBuilder.Verify();

            var auditor = new BusAuditor(_host.GetStorage<IStoreAudits>());

          
          
            _host.Build(container,auditor);

            var dispatcher = _dispBuilder.BuildClient(_host.HostName,_host.GetStorage<IDeliveryErrorsQueue>(),auditor);                                       

            dispatcher.SubscribeToServer(_host.Endpoints);

            _bus.Init(dispatcher,_host,_dispBuilder.Receiver);                                           
            _bus.StartProcessors();
            _bus.StartListeningForMessages();
            
            return _bus;
        }

    
      

       
    }

    

    
}