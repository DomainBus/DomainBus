using System;
using System.Collections.Generic;
using System.Reflection;
using CavemanTools.Infrastructure;
using CavemanTools.Logging;
using DomainBus.Abstractions;

namespace DomainBus.Processing.Internals
{
    public class SagaExecutor : IInvokeHandler
    {
        protected readonly IHandlerTypeInvoker HandlerInvoker;

        protected Type SagaStateType;
        protected IContainerScope Container;
     
        public SagaExecutor(IHandlerTypeInvoker handlerInvoker, IContainerScope container)
        {
            HandlerInvoker = handlerInvoker;

            Container = container;
            ExtractSagaType();
        }

        private void ExtractSagaType()
        {
            var sagaType = HandlerInvoker.HandlerType;
            if (!sagaType.InheritsGenericType(typeof (Saga<>)))
            {
                throw new InvalidOperationException(
              "{0} is not a valid saga. Sagas must derive from Saga<T>".ToFormat(sagaType.FullName));
            }
            var baseType = sagaType.GetTypeInfo().BaseType;
            
            SagaStateType = baseType.GetGenericArguments()[0];
          
          
        }

        internal bool UseCustomRepositories { get; set; }


        ISagaState GetSagaStateFromCustom(IEvent evnt, IResolveDependencies resolver)
        {
            var evntType = evnt.GetType();
            dynamic repo =
                resolver.ResolveOptional(typeof(IFindSagaState<>.Using<>).MakeGenericType(SagaStateType, evntType));
            if (repo == null)
            {
                return null;
            }

            string fullName = repo.GetType().FullName;
            this.LogDebug($"Using saga repository '{fullName}' ...");
            return repo.GetSagaData((dynamic) evnt);
        }

        protected ISagaState GetSagaState(IEvent evnt, IResolveDependencies resolver, string correlationId)
        {
            evnt.MustNotBeNull();
          
           this.LogDebug($"Loading saga {SagaStateType.Name} ...");

            ISagaState res = null;

            if (UseCustomRepositories)
            {
                res = GetSagaStateFromCustom(evnt, resolver);
                if (res != null) return res;
            }
            
            var grepo = GetSagaRepository(resolver);
            
            return grepo.GetSaga(correlationId,SagaStateType);
        }

        private IStoreSagaState GetSagaRepository(IResolveDependencies resolver)
        {
            IStoreSagaState grepo = null;
            try
            {
                grepo = resolver.Resolve<IStoreSagaState>();
                this.LogDebug($"Using saga repository '{ grepo.GetType()}' ...");
            }
            catch (Exception ex)
            {
                throw new DiContainerException(typeof(IStoreSagaState),ex);
            }
            return grepo;
        }

        protected void SaveSagaState(dynamic saga, IResolveDependencies resolver, string id,bool isNew=false)
        {
            if (UseCustomRepositories && SaveSagaToCustom(saga, resolver, isNew)) return;

            var grepo = GetSagaRepository(resolver);
            grepo.Save(saga, id, isNew);

            this.LogDebug("Saga data '{0}' saved", SagaStateType);
        }

        private bool SaveSagaToCustom(dynamic saga, IResolveDependencies resolver, bool isNew)
        {
            dynamic repo = resolver.ResolveOptional(typeof (ISaveSagaState<>).MakeGenericType(SagaStateType));

            this.LogDebug("Saving saga {0} ...", SagaStateType.Name);

            if (repo == null) return false;

            string fullName = repo.GetType().FullName;
            this.LogDebug("Using repository '{0}'", fullName);
            repo.Save(saga, isNew);
            return true;
        }

        public virtual void Handle(IMessage msg,string processor)
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                dynamic handler = HandlerInvoker.InstantiateHandler(scope);

                var correlationId = handler.GetCorrelationId(msg as IEvent);
                var tries = 0;
                while (tries < 5)
                {
                     LogManager.LogDebug(this, "Loading saga {0} with correlation id {1}", SagaStateType.Name, correlationId);
                     dynamic data =  GetSagaState(msg as IEvent, scope, correlationId);

                    if (data == null || data.IsCompleted)
                     {
                         this.LogInfo("Saga '{0}' completed or not found. Ignoring message '{1}'",
                                      SagaStateType.FullName, msg.GetType().FullName);
                         return;
                     }

                     string stateName = data.GetType().ToString();
                     this.LogDebug("Handling saga '{2}' using saga state '{0}' with correlation id {1}", stateName, (string)correlationId, SagaStateType.Name);
                     handler.Data = data;
                      HandlerInvoker.HandleMessage(msg, handler,processor);
                    try
                    {
                         SaveSagaState(data, scope, correlationId);
                        break;
                    }
                    catch (SagaConcurrencyException)
                    {
                        tries++;
                        this.LogInfo("Saga '{0}' was already updated.  Retrying...", SagaStateType.FullName);
                    }
                }
                if (tries >= 5)
                {
                    throw new SagaExecutorException("Could no save saga state after 5 tries. Aborting.");
                }
               
            }
        }

        public IEnumerable<Type> GetHandlersTypes()
        {
            return HandlerInvoker.GetHandlersTypes();
        }
    }
}