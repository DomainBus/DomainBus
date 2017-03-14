using System;
using System.Collections.Generic;
using CavemanTools.Infrastructure;
using DomainBus.Audit;
using DomainBus.Configuration.Internals;
using DomainBus.Processing;
using DomainBus.Transport;

namespace DomainBus.Configuration
{
   
    public abstract class AbstractBusBuilder:IBuildBusWithContainer
    {
       
        private DispatcherBuilder _dispBuilder;
        private ConfigureHost _host;


        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        protected abstract void RegisterSingletonInstance<T>(T instance);
       
        /// <summary>
        /// Should always register types as self and as implemented interfaces.
        /// </summary>
        /// <param name="types"></param>
        protected abstract void Register(IEnumerable<Type> types);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        protected abstract void RegisterInstanceFactory<T>(Func<T> instance);

        /// <summary>
        /// All modifications after BuildContainerScope() need to update the built container.
        /// Basically, we need to keep the same container reference
        /// </summary>
        protected abstract void ContainerConfigurationCompleted();
        
        /// <summary>
        /// We need a first container build. This will be updated later
        /// </summary>
        /// <returns></returns>
        protected abstract IContainerScope BuildContainerScope();

        public IBuildBusWithContainer ServerComunication(Action<IConfigureDispatcher> cfg)
        {
            _dispBuilder = new DispatcherBuilder();
            cfg(_dispBuilder);
            return this;
        }

        public IBuildBusWithContainer CurrentHost(Action<IConfigureHost> cfg)
        {
            cfg.MustNotBeNull();
            _host = new ConfigureHost();
            cfg(_host);
            return this;
        }

        
      

        void RegisterTypesInContainer()
        {
            RegisterSingletonInstance(new BusAuditor(_host.GetStorage<IStoreAudits>()));
            RegisterSingletonInstance(_host.GetStorage<IStoreSagaState>());
            
            _host.Handlers.MustNotBeNull("Handlers are null");            
            _host.SagaStateTypes.MustNotBeNull("Saga state types are null");
            Register(_host.Handlers);        
            Register(_host.SagaStateTypes);
        }

        public IDomainBus Build(bool start = true)
        {
            _dispBuilder.MustNotBeNull("Dispatcher must be configured");
            _host.MustNotBeNull("Processors must be configured");

            _host.VerifyWeHaveAll();
            _dispBuilder.Verify();

            var auditor = new BusAuditor(_host.GetStorage<IStoreAudits>());


            RegisterTypesInContainer();
            var container = BuildContainerScope();
            _host.Build(container,auditor);

            var dispatcher = _dispBuilder.BuildClient(_host.HostName,_host.GetStorage<IDeliveryErrorsQueue>(),auditor);                            

           

            dispatcher.SubscribeToServer(_host.Endpoints);

            var d= new ServiceBus(container,dispatcher,_host,_dispBuilder.Receiver);
            RegisterSingletonInstance(d);
            RegisterInstanceFactory(d.GetDispatcher);
            ContainerConfigurationCompleted();
            if (start)
            {
                d.StartProcessors();
                d.StartListeningForMessages();
            }
            return d;
        }

    
      

       
    }

    
}