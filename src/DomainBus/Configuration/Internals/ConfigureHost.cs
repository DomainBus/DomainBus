using System;
using System.Collections.Generic;
using System.Linq;
using CavemanTools.Infrastructure;
using DomainBus.Audit;
using DomainBus.Dispatcher;
using DomainBus.Dispatcher.Client;
using DomainBus.Processing;
using DomainBus.Repositories;
using DomainBus.Transport;

namespace DomainBus.Configuration.Internals
{
    class ConfigureHost : IConfigureHost,IConfigureSagas
    {

        public ConfigureHost()
        {
           RequiredStorages.ForEach(t=>_storages[t]=NullStorage.Instance);            
        }

        public static readonly Type[] RequiredStorages =
        {
            typeof(IStoreUnhandledMessages)
            ,typeof(IStoreAudits)
            ,typeof(IStoreReservedMessagesIds)
            ,typeof(IStoreSagaState)
            ,typeof(IFailedMessagesQueue)
            ,typeof(IDeliveryErrorsQueue)
                
        };

        #region Implementation of IConfigureHost

        readonly Dictionary<Type,object> _storages=new Dictionary<Type, object>();

        public T GetStorage<T>() where T:class
        {
            var storage = _storages.GetValueOrDefault(typeof(T)) as T;
            storage.MustNotBeNull("Configuration bug, this should always have a value");
            
            return storage;
        }

        public string HostName => _hostName;

        public ProcessorConfig Processors { get; } = new ProcessorConfig();

        public List<Type> Handlers { get; } = new List<Type>();

        IConfigureHost IConfigureHost.WithProcessingStorage(IStoreUnhandledMessages store)
        {
            store.MustNotBeNull();
            _storages[typeof (IStoreUnhandledMessages)] = store;
            return this;
        }

        IConfigureHost IConfigureHost.WithReserveIdStorage(IStoreReservedMessagesIds store)
        {
            store.MustNotBeNull();
            _storages[typeof (IStoreReservedMessagesIds)] = store;
                return this;
        }

        IConfigureHost IConfigureHost.SendFailedMessagesTo(IFailedMessagesQueue store)
        {
            store.MustNotBeNull();
            _storages[typeof(IFailedMessagesQueue)] = store;
            return this;
        }

        IConfigureHost IConfigureHost.SendFailedDeliveriesTo(IDeliveryErrorsQueue queue)
        {
            queue.MustNotBeNull();
            _storages[typeof(IDeliveryErrorsQueue)] = queue;
            return this;
        }

        IConfigureHost IConfigureHost.PersistAuditsWith(IStoreAudits store)
        {
            store.MustNotBeNull();
            _storages[typeof(IStoreAudits)] = store;
            return this;
        }

        private string _hostName;

        IConfigureHost IConfigureHost.HostnameIs(string name)
        {
            name.MustNotBeEmpty();
            _hostName = name;
            return this;
        }

        IConfigureHost IConfigureHost.ConfigureSagas(Action<IConfigureSagas> cfg)
        {
            cfg(this);
            return this;
        }

        public IConfigureHost AutoConfigureFrom(IEnumerable<Type> types)
        {
            var t = types?.ToArray()??Type.EmptyTypes;
            AddHandlers(t);
            AddSagaStates(t);
            return this;
        }

        /// <exception cref="DomainBusConfigurationException">When missing a storage implemementation.</exception>
        internal void VerifyWeHaveAll()
        {
            _hostName.MustNotBeEmpty();
            Processors.Verify();
            var missing=_storages
                .Where(kv => kv.Value == null)
                .Select(d => d.Key).ToArray();
            if (!missing.Any()) return;
            throw new DomainBusConfigurationException($"I need an implementation for the following interfaces: '{missing.StringJoin()}'");
           
        }

        public IEnumerable<Type> SagaStateTypes { get; private set; }  =Enumerable.Empty<Type>();

        private void AddSagaStates(Type[] types)
            => SagaStateTypes = types.Where(t => t.IsSagaState()).ToArray();

        void AddHandlers(IEnumerable<Type> handlers) => Handlers.AddRange(handlers.Where(d=>d.IsMessageHandler()));
        

      
        IConfigureHost IConfigureHost.ConfigureProcessors(Action<IConfigureProcessors> cfg)
        {
            cfg.MustNotBeNull();
            cfg(Processors);
            return this;
        }

        public EndpointConfig[] Endpoints { get; private set; }

        public void Build(IContainerScope container,BusAuditor auditor)
        {
            Endpoints=Processors.Build(this, container, auditor);
        }

        #endregion

        #region Implementation of IConfigureSagas

        internal bool UseUserDefinedSagaRepos;

        IConfigureSagas IConfigureSagas.EnableUserDefinedRepositories()
        {
            UseUserDefinedSagaRepos = true;
            return this;
        }

        IConfigureSagas IConfigureSagas.WithSagaStorage(IStoreSagaState store)
        {
            _storages[typeof (IStoreSagaState)] = store;
            return this;
        }

        #endregion
    }
}