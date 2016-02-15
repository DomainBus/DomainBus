using System;
using System.Collections.Generic;
using CavemanTools.Infrastructure;
using DomainBus.Abstractions;
using DomainBus.Audit;

namespace DomainBus.Processing.Internals
{
    public class HandlerTypeInvoker : IInvokeHandler, IHandlerTypeInvoker
    {
        private readonly Type _handlerType;
        private readonly IContainerScope _container;
        private readonly BusAuditor _auditor;
        private readonly IFailedMessagesQueue _errors;

        public HandlerTypeInvoker(Type handlerType, IContainerScope container,BusAuditor auditor,IFailedMessagesQueue errors)
        {
            handlerType.MustNotBeNull();
            container.MustNotBeNull();
            _handlerType = handlerType;
            _container = container;
            _auditor = auditor;
            _errors = errors;
        }

        public Type HandlerType => _handlerType;

        public void HandleMessage(IMessage msg, dynamic handler,string processor)
        {
           if(msg.IsNot<ICommand>() && msg.IsNot<IEvent>()) return;
            _auditor.Handling(msg, HandlerType, processor);
         
            try
            {
                if (msg is ICommand) handler.Execute((dynamic) msg);
                else
                {
                    handler.Handle((dynamic) msg);
                }

            }
            catch (Exception ex)
            {
                _auditor.Handled(msg, HandlerType, processor,ex);
                var mex= new HandledMessageException(handler.GetType(),msg,ex);
                _errors.MessageHandlingFailed(msg,mex);
              
              return;
              
            }

            //just in case the auditor throws
            
            _auditor.Handled(msg, HandlerType, processor);
                    
         
        }

        public void Handle(IMessage msg,string processor)
        {
            using (var resolver = _container.BeginLifetimeScope())
            {
                var handler = InstantiateHandler(resolver);
                HandleMessage(msg, handler,processor);
            }
        }

        public dynamic InstantiateHandler(IContainerScope resolver)
        {
            dynamic handler = null;
            try
            {
                handler = resolver.Resolve(HandlerType);
            }
            catch (Exception ex)
            {
                throw new DiContainerException(HandlerType, ex);
            }

            if (handler == null)
            {
                throw new DiContainerException(HandlerType, null);
            }
            return handler;
        }

        public IEnumerable<Type> GetHandlersTypes()
        {
            return new []{HandlerType};
        }
    }
}