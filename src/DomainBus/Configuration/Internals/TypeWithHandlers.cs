using System;
using System.Collections.Generic;

namespace DomainBus.Configuration.Internals
{
    public class TypeWithHandlers
    {
        private readonly Type _handlerType;
        private readonly IEnumerable<Type> _messagesTypes;

        public TypeWithHandlers(Type handlerType, IEnumerable<Type> messagesTypes)
        {
            _handlerType = handlerType;
            _messagesTypes = messagesTypes;
        }

        public IEnumerable<Type> MessagesTypes => _messagesTypes;

        public Type HandlerTypeName => _handlerType;

        /// <summary>
        /// Returns null if the type isn't a handler
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TypeWithHandlers TryCreateFrom(Type t) 
            => MessageHandlerDiscoverer.GetHandledMessageTypes(t).IsNullOrEmpty()
            ? null 
            : new TypeWithHandlers(t, MessageHandlerDiscoverer.GetHandledMessageTypes(t));

        public static bool IsHandler(Type t) => t.CanBeInstantiated() && MessageHandlerDiscoverer.IsHandlerType(t);
    }
}