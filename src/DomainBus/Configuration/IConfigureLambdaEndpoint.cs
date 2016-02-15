using System;

namespace DomainBus.Configuration
{
    public interface IConfigureLambdaEndpoint
    {
        /// <summary>
        /// Add only the handlers you want to be executed by this processor
        /// </summary>
        /// <param name="handlerTypes"></param>
        /// <returns></returns>
        IConfigureLambdaEndpoint HandleOnly(params Type[] handlerTypes);

        /// <summary>
        /// Handlers are part of a namespace which contains the specified name
        /// </summary>
        /// <param name="nspace">If null then it's the processor's name</param>
        /// <returns></returns>
        IConfigureLambdaEndpoint HandleFromNamespace(string nspace=null);

        /// <summary>
        /// Handlers have the <see cref="DomainBusAttribute"/> with name of the processor or another specified name
        /// </summary>
        /// <param name="name">Attribute value</param>
        /// <returns></returns>
        IConfigureLambdaEndpoint HandleAtributeDecorated(string name = null);

        /// <summary>
        /// Which handlers from assemblies will be executed by this processor
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        IConfigureLambdaEndpoint Handle(Predicate<Type> match);

        /// <summary>
        /// All handlers will be executed by this processor
        /// </summary>
        /// <returns></returns>
        IConfigureLambdaEndpoint HandlesEverything();
    }
}