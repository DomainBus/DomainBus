using System;
using System.Collections.Generic;

namespace DomainBus.Configuration
{
   
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
        void Register(Type[] types, bool asSingleton = false);

        /// <summary>
        ///  Register lambda as factory for specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        void RegisterInstanceFactory<T>(Func<T> instance);
        
    }


}