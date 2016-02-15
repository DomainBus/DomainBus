using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DomainBus.Configuration.Internals
{
    public class InterfaceBasedDiscovery:IDiscoverHandlers
    {
        private static readonly Type[] InterfaceTypes = { typeof(IExecute<>), typeof(ISubscribeTo<>), typeof(IAmStartedBy<>) };
        #region Implementation of IDiscoverHandlers

        public IEnumerable<Type> GetHandledMessageTypes(Type type)
            =>
                type.GetTypeInfo().ImplementedInterfaces
                    .Where(d => IntrospectionExtensions.GetTypeInfo(d).IsInterface)
                    .Where(i => InterfaceTypes.Any(d => d.Name == i.Name))
                    .Select(i => i.GenericTypeArguments[0])
                    .Distinct();
        

        public bool IsHandler(Type type)
            => InterfaceTypes.Any(t => type.ImplementsGenericInterface(t));

        public bool CanStartSaga(Type type, Type msgType) => type.ImplementsGenericInterface(typeof(IAmStartedBy<>), msgType);

        #endregion
    }
}