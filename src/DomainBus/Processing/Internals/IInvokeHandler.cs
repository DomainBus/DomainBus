using System;
using System.Collections.Generic;
using DomainBus.Abstractions;

namespace DomainBus.Processing.Internals
{
    public interface IInvokeHandler
    {
        void Handle(IMessage msg,string processor);
        IEnumerable<Type> GetHandlersTypes();
    }
}