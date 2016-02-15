using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DomainBus.Configuration.Internals
{
    public class MethodBasedDiscovery : IDiscoverHandlers
    {
        private static readonly string[] Methods = {"Execute","Handle"};

        #region Implementation of IDiscoverHandlers

        public IEnumerable<Type> GetHandledMessageTypes(Type type)
            => type.GetMethods().Where(IsHandlerMethod).Select(GetHandledMessageType);

        static bool IsHandlerMethod(MethodInfo info)
        {
            if (!Methods.Contains(info.Name) || info.ReturnType!=typeof(void)) return false;
            var p = info.GetParameters();
            if (p.Length != 1) return false;
            var ptype = p[0].ParameterType;
            if (info.Name == "Execute" && ptype.Implements<ICommand>()) return true;
            if (info.Name == "Handle" && ptype.Implements<IEvent>()) return true;
            return false;
        }

        static Type GetHandledMessageType(MethodInfo info)
            => info.GetParameters()[0].ParameterType;
       


        public bool IsHandler(Type type)
            => type.GetMethods().Any(IsHandlerMethod);
        

        public bool CanStartSaga(Type type, Type msgType) 
            => type.GetMethods().Any(mi=>
                IsHandlerMethod(mi) 
                && mi.HasCustomAttribute<StartsSagaAttribute>() 
                && GetHandledMessageType(mi)==msgType);

        #endregion
    }
}