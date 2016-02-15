using System;
using System.Collections.Generic;
using CavemanTools.Infrastructure;
using DomainBus.Abstractions;

namespace DomainBus.Processing.Internals
{
    public interface IHandlerTypeInvoker
    {
        void HandleMessage(IMessage msg, dynamic handler,string processor);
        dynamic InstantiateHandler(IContainerScope resolver);
        Type HandlerType { get; }
        IEnumerable<Type> GetHandlersTypes();
    }
}