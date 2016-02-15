using System;
using System.Collections.Generic;
using System.Linq;
using CavemanTools.Infrastructure;
using CavemanTools.Logging;
using DomainBus.Audit;
using DomainBus.Processing;
using DomainBus.Processing.Internals;

namespace DomainBus.Configuration.Internals
{
    /// <summary>
    /// Each processor has its own
    /// </summary>
    internal class MessageHandlersNexus : IKnowMessageHandlers
    {
        private const string LoggingName = "DomainBus";
        private readonly IContainerScope _container;
        private readonly BusAuditor _auditor;
        private readonly ConfigureHost _host;
        private readonly IFailedMessagesQueue _error;
        private readonly Dictionary<Type, IInvokeHandler> _handlers = new Dictionary<Type, IInvokeHandler>();

        public MessageHandlersNexus(IContainerScope container,BusAuditor auditor,ConfigureHost host)
        {
            container.MustNotBeNull();
            _container = container;
            _auditor = auditor;
            _host = host;
            _error = host.GetStorage<IFailedMessagesQueue>();
        }

        public Dictionary<Type, IInvokeHandler> Handlers => _handlers;

        public IEnumerable<Type> GetHandlersTypes() => _handlers.Values.SelectMany(d=>d.GetHandlersTypes());

        public IEnumerable<Type> GetMessageTypes() => _handlers.Keys;

        /// <summary>
        /// Types not handling messages are ignored
        /// </summary>
        /// <param name="handlerTypes"></param>
        public void Add(params Type[] handlerTypes)
        {
            if (handlerTypes.Length==0) return;
            foreach (var handlerType in handlerTypes)
            {
                var th = TypeWithHandlers.TryCreateFrom(handlerType);
                if (th == null) continue;
                foreach (var msg in th.MessagesTypes)
                {
                    if (msg.Implements<ICommand>())
                    {
                        AddCommand(msg, handlerType);
                        continue;
                    }

                    if (msg.Implements<IEvent>())
                    {
                        AddEvent(msg, handlerType);                  
                    }
                }
            }
            
        }

        private void AddEvent(Type msg, Type handler)
        {
            IInvokeHandler invoker;
            EventHandlerSubscription sub;
            if (_handlers.TryGetValue(msg, out invoker))
            {
                sub = invoker as EventHandlerSubscription;
            }
            else
            {
                sub = new EventHandlerSubscription();
                _handlers.Add(msg, sub);
            }

            if (handler.IsSaga())
            {
                AddSagaEvent(handler, msg, sub);
            }
            else
            {
                sub.Add(new HandlerTypeInvoker(handler, _container,_auditor,_error));
            }
            LoggingName.LogDebug("Added handler '{0}' for event '{1}'", handler.FullName, msg.FullName);
        }

        private void AddSagaEvent(Type handler, Type msg, EventHandlerSubscription sub)
        {
            var invoker = new HandlerTypeInvoker(handler, _container,_auditor,_error);
            SagaExecutor executor = null;
            if (msg.CanStartSaga(handler))
            {
                executor = new SagaStarterExecutor(invoker, _container);             
            }
            else
            {
                executor = new SagaExecutor(invoker, _container);
             
            }
            executor.UseCustomRepositories = _host.UseUserDefinedSagaRepos;
            sub.Add(executor);

        }

        private void AddCommand(Type msg, Type handler)
        {
            if (_handlers.ContainsKey(msg))
            {
                throw new DuplicateCommandHandlerException();
            }
            _handlers.Add(msg, new HandlerTypeInvoker(handler, _container, _auditor, _error));
            LoggingName.LogDebug("Added handler '{0}' for command '{1}'", handler.FullName, msg.FullName);
        }

        public IInvokeHandler GetHandlerInvoker(Type msgType)
            => Handlers.GetValueOrDefault(msgType);
        

    }
}