using System;
using System.Collections.Generic;

namespace DomainBus.Processing.Internals
{
    public interface IKnowMessageHandlers
    {
        /// <summary>
        /// Returns a handler invoker or null
        /// </summary>
        /// <param name="msgType"></param>
        /// <returns></returns>
        IInvokeHandler GetHandlerInvoker(Type msgType);

        IEnumerable<Type> GetHandlersTypes();

        IEnumerable<Type> GetMessageTypes();
    }
}