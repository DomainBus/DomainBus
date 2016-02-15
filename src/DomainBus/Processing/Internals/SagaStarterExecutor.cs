using CavemanTools.Infrastructure;
using CavemanTools.Logging;
using DomainBus.Abstractions;

namespace DomainBus.Processing.Internals
{
    public class SagaStarterExecutor : SagaExecutor
    {
        public SagaStarterExecutor(IHandlerTypeInvoker handlerInvoker, IContainerScope container) : base(handlerInvoker, container)
        {
        }

        public override void Handle(IMessage msg,string processor)
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                dynamic saga = HandlerInvoker.InstantiateHandler(scope);

                var correlationId = saga.GetCorrelationId(msg as IEvent);

                dynamic sagaState = scope.Resolve(SagaStateType);

                this.LogDebug($"Message '{msg}' started new saga {SagaStateType}");
                
                saga.Data = sagaState;
                HandlerInvoker.HandleMessage(msg, saga, processor);

                try
                {
                    SaveSagaState(sagaState, scope, correlationId, true);

                }
                catch (SagaExistsException)
                {
                    this.LogInfo($"Saga '{SagaStateType.FullName}' already exists. We leave it there");
                }
            }

        }
    }
}