using System;
using System.Collections.Generic;
using CavemanTools.Infrastructure;

namespace DomainBus.Configuration
{
    //public interface IBuildBusWithContainer
    //{
      
    //    IBuildBusWithContainer ServerComunication(Action<IConfigureDispatcher> cfg);

    //    IBuildBusWithContainer CurrentHost(Action<IConfigureHost> cfg);

      
    //}

    public interface IRegisterBusTypesInContainer
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        void RegisterSingletonInstance<T>(T instance);

        /// <summary>
        /// Should always register types as self and as implemented interfaces.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="asSingleton"></param>
        void Register(IEnumerable<Type> types, bool asSingleton = false);

        /// <summary>
        ///  Register lambda as factory for specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        void RegisterInstanceFactory<T>(Func<T> instance);
        
    }


}