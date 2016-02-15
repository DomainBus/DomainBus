using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DomainBus.Configuration
{
    public class HostedEndpointConfiguration:IConfigureHostedEndpoint,IConfigureLambdaEndpoint
    {
        protected Assembly ThisAssembly
        {
            get
            {
                //var thisType = GetType();
                //if (thisType.GetTypeInfo().BaseType != typeof(HostedEndpointConfiguration))
                //    throw new InvalidOperationException("This property is available only for ");

                return GetType().GetTypeInfo().Assembly;
            }
        }

        public HostedEndpointConfiguration(string name):this()
        {
           name.MustNotBeEmpty();           
            Name = name;
        }

        protected HostedEndpointConfiguration()
        {
             
        }

        protected void NameFromType() => Name = GetType().Name;

        public IConfigureLambdaEndpoint HandleOnly(params Type[] handlerTypes)
        {
            Handle(handlerTypes.Contains);
            return this;
        }

        /// <summary>
        /// Register handlers having the name of the endpoint in the namespace,from the given assemblies.
        /// </summary>
        /// <param name="asms"></param>
        public void AddFromCurrentName(params Assembly[] asms)
        {
            HandleFromNamespace(GetType().Name);
        }

        public IConfigureLambdaEndpoint HandleFromNamespace(string nspace=null)
        {
            Handle(t=>t.Namespace.Contains(nspace??Name));
            return this;
        }

        public IConfigureLambdaEndpoint HandleAtributeDecorated(string name = null)
        {
            name = name ?? Name;
            Handle(t=>t.GetTypeInfo().GetAttributeValue<DomainBusAttribute,string>(a=>a.EndpointName)==name);
            return this;
        }

        List<Predicate<Type>> _strategies=new List<Predicate<Type>>();

        public IConfigureLambdaEndpoint Handle(Predicate<Type> match)
        {
            _strategies.Add(match);
            return this;
        }

        public IConfigureLambdaEndpoint HandlesEverything()
        {
            Handle(t => true);
            return this;
        }





        public bool CanHandle(Type type)
            => _strategies.Any(s => s(type));

        public string Name { get; set; }
    }
}