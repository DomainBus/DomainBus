using System;
using System.Linq;
using System.Reflection;
using DomainBus.Abstractions;
using DomainBus.Audit;
using DomainBus.Configuration.Internals;
using DomainBus.Dispatcher.Client;
using DomainBus.Processing;
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
        /// Handlers are types decorated with <see cref="DomainBusAttribute"/> from the specified assemblies 
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
        public static IConfigureDispatcher LocalDispatcher(this IConfigureDispatcher cfg) => cfg.TalkUsing(NullServerConnector.Instance).ReceiveMessagesUsing(NullReceiver.Instance);
        
            
        /// <summary>
        /// ONLY for development/debugging scenarios
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IConfigureHost WithInMemoryAudits(this IConfigureHost host)
            => host.PersistAuditsWith(new InMemoryAuditStorage());

        
        /// <summary>
        /// Autoconfiguration from types of the provided assemblies
        /// </summary>
        /// <param name="host"></param>
        /// <param name="asms"></param>
        /// <returns></returns>
        public static IConfigureHost RegisterHandlersAndSagaStatesFrom(this IConfigureHost host, params Assembly[] asms)
            =>
                host.AutoConfigureFrom(asms.SelectMany(a => a.GetExportedTypes()));

       
        /// <summary>
        /// Sets host name to 'local'. For monolith mode
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IConfigureHost LocalHost(this IConfigureHost host)
            => host.HostnameIs("local");

       

      
       

    }
}