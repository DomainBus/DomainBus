using System;
using System.Collections.Generic;

namespace DomainBus.Configuration.Internals
{
    public static class MessageHandlerDiscoverer
    {
        
       public static IDiscoverHandlers Discoverer=new MethodBasedDiscovery();


        public static bool IsHandlerType(Type type) => Discoverer.IsHandler(type);

        public static IEnumerable<Type> GetHandledMessageTypes(Type type)
            => Discoverer.GetHandledMessageTypes(type);
           


        public static bool CanStartSaga(this Type msgType, Type tp) =>Discoverer.CanStartSaga(tp,msgType);

        public static bool IsSaga(this Type tp) => tp.InheritsGenericType(typeof (Saga<>));
    }

    public interface IDiscoverHandlers
    {
        IEnumerable<Type> GetHandledMessageTypes(Type type);
        bool IsHandler(Type type);
        bool CanStartSaga(Type type,Type msgType);
    }
}