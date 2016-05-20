using System;
using System.Linq;
using System.Reflection;
using DomainBus.Abstractions;
using DomainBus.Audit;
using DomainBus.Configuration.Internals;
using DomainBus.Dispatcher.Client;
using DomainBus.Processing;
using DomainBus.Repositories;
using DomainBus.Transport;

namespace DomainBus.Configuration
{
   
    public static class BusBuilderExtensions
    {
        private static readonly Type[] SagaStorageTypes = new[] {typeof (IStoreSagaState), typeof (ISaveSagaState<>), typeof (IFindSagaState<>.Using<>)};

        public static string ToFlatName(this IMessage msg)
        {
            msg.MustNotBeNull();
            return msg.GetType().AsMessageName();
        }

        public static string AsMessageName(this Type msgType) => msgType.FullName;

        public static bool IsEvent(this Type msgType) => msgType.Implements<IEvent>();

        public static bool IsCommand(this Type msgType) => msgType.Implements<ICommand>();


        public static bool IsMessageHandler(this Type type) => TypeWithHandlers.IsHandler(type);
        public static bool IsSagaState(this Type type) => type.Implements<ISagaState>() && !type.GetTypeInfo().IsAbstract;

        public static bool IsSagaRelatedStorage(this Type type)
            =>SagaStorageTypes.Contains(type);

        public static bool IsUserSagaRepository(this Type type)
        {
            if (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface) return false;
            if (type.ImplementsGenericInterface(typeof(ISaveSagaState<>))) return true;
            return type.ImplementsGenericInterface(typeof (IFindSagaState<>.Using<>));
           
        }

        /// <summary>
        /// Handlers are types from the specified assemblies decorated with <see cref="DomainBusAttribute"/>
        /// </summary>
        /// <param name="procs"></param>
        /// <param name="name"></param>
        /// <param name="assembly"></param>
        /// <param name="svcCfg"></param>
        /// <returns></returns>
        public static IConfigureProcessors AddWithAttributeConvention(this IConfigureProcessors procs, string name,
            Assembly assembly, Action<IConfigureProcessingService> svcCfg = null)
            => procs.Add(name, c => c.HandleAtributeDecorated(), svcCfg);

        /// <summary>
        /// Adds a processor which must be configured with the handlers the processor should execute
        /// </summary>
        /// <param name="procs"></param>
        /// <param name="name">Processor name</param>
        /// <param name="cfg">Handlers discovery</param>
        /// <param name="svcCfg"></param>
        /// <returns></returns>
        public static IConfigureProcessors Add(this IConfigureProcessors procs,string name,Action<IConfigureLambdaEndpoint> cfg,Action<IConfigureProcessingService> svcCfg=null)
        {
            cfg.MustNotBeNull();
            var proc=new HostedEndpointConfiguration(name);
            cfg(proc);
            procs.ForEndpoint(proc, svcCfg);
            return procs;
        }

        public static string GetProcessorFullName(this IDomainBus bus,string name) => bus.GetProcessingQueue(name).Name;

        /// <summary>
        /// Doesn't communicate with the server
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static IConfigureDispatcher LocalDispatcher(this IConfigureDispatcher cfg) => cfg.TalkUsing(NullServerConnector.Instance);

       
      

      
        /// <summary>
        /// Configures the domain bus strictly as a (non-durable)memory bus. Idempotency is not enforced and sagas shouldn't be used
        /// </summary>
        /// <param name="build"></param>
        /// <returns></returns>
        public static IDomainBus AsMemoryBus(this IBuildBusWithContainer build, params Assembly[] assemblies) 
            => build.AsMemoryBus(assemblies.SelectMany(a => a.GetExportedTypes().Where(IsMessageHandler)).ToArray());

       
        /// <summary>
        /// Configures the domain bus strictly as a (non-durable)memory bus. Idempotency is not enforced and sagas shouldn't be used
        /// </summary>
        /// <param name="build"></param>
        /// <param name="handlerTypes">handlers used by memory bus</param>
        /// <returns></returns>
        public static IDomainBus AsMemoryBus(this IBuildBusWithContainer build, params Type[] handlerTypes)
            => build.ForMonolith(c =>
           
                c.HostnameIs("local")
                .AutoConfigureFrom(handlerTypes)
                .ConfigureSagas(s=>s.WithSagaStorage(NullStorage.Instance))
                .PersistAuditsWith(NullStorage.Instance)
                .SendFailedDeliveriesTo(NullStorage.Instance)
                .SendFailedMessagesTo(NullStorage.Instance)
                .WithProcessingStorage(NullStorage.Instance)
                .WithReserveIdStorage(NullStorage.Instance)
                .ConfigureProcessors(
                    procs=>procs.Add("memorybus",endpoint=>endpoint.HandleOnly(handlerTypes)))
           );

        /// <summary>
        /// ONLY for development/debugging scenarios
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IConfigureHost StoreAuditsInMemory(this IConfigureHost host)
            => host.PersistAuditsWith(new InMemoryAuditStorage());

        public static IConfigureHost WithoutSagas(this IConfigureHost host)
            => host.ConfigureSagas(d => d.WithoutSagas());
        /// <summary>
        /// Autoconfiguration from types of the provided assemblies
        /// </summary>
        /// <param name="host"></param>
        /// <param name="asms"></param>
        /// <returns></returns>
        public static IConfigureHost RegisterHandlersAndSagasFrom(this IConfigureHost host, params Assembly[] asms)
            =>
                host.AutoConfigureFrom(asms.SelectMany(a => a.GetExportedTypes()));


        public static IConfigureHost WithoutAuditor(this IConfigureHost host)
            => host.PersistAuditsWith(NullStorage.Instance);

        public static IConfigureHost LocalHost(this IConfigureHost host)
            => host.HostnameIs("local");

        public static IConfigureHost WithoutErrorsQueues(this IConfigureHost host)
            => host.SendFailedDeliveriesTo(NullStorage.Instance).SendFailedMessagesTo(NullStorage.Instance);

        public static IConfigureSagas WithoutSagas(this IConfigureSagas host)
            => host.WithSagaStorage(NullStorage.Instance);

        /// <summary>
        /// Bus works only inside an application
        /// </summary>
        /// <param name="build"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IDomainBus ForMonolith(this IBuildBusWithContainer build, Action<IConfigureHost> config)
            =>
                build
                    .ServerComunication(
                        c => c.ReceiveMessagesUsing(NullReceiver.Instance).TalkUsing(NullServerConnector.Instance))
                    .CurrentHost(d =>
                    {
                        config(d.LocalHost());
                    })
                    .Build();

    }
}