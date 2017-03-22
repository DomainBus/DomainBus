using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using CavemanTools;
using DomainBus.Processing;

namespace DomainBus
{
    /// <summary>
    /// Base class for sagas
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Saga<T> where T : ISagaState
    {
        public T Data { get; internal set; }


        public void MarkAsCompleted()
        {
            Data.IsCompleted = true;
            OnComplete(Data);
        }

        internal Func<T, bool> Completion { get; set; } = d => false;

        internal Action<T> OnComplete { get; set; } = Empty.ActionOf<T>();

        /// <summary>
        /// Gets the correlation id for saga based on the mapped event property and value.
        ///  </summary>
        /// <exception cref="SagaConfigurationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <param name="evnt"></param>
        /// <returns></returns>
        internal string GetCorrelationId(IEvent evnt)
        {
            evnt.MustNotBeNull();


            PropertyInfo pi = null;
            var etype = evnt.GetType();
            if (!_maps.TryGetValue(etype, out pi))
            {
                throw new SagaConfigurationException(
                    "No mapping done for event '{0}'. Call CorrelationIdFor<> in the constructor".ToFormat(evnt.GetType().FullName));
            }

            var value = evnt.GetPropertyValue(pi.Name);

            if (value == null || (pi.PropertyType.IsValueType() && value.Equals(pi.PropertyType.GetDefault())))
            {
                throw new NullReferenceException("'{0}' property of '{1}' must have a non-default value".ToFormat(pi.Name, etype));
            }

            return FormatId(value.ToString());
        }

     

        private readonly Dictionary<Type, PropertyInfo> _maps = new Dictionary<Type, PropertyInfo>();

      

        private string FormatId(string value) => $"{value}-{typeof(T).FullName}";


        /// <summary>
        /// Which event property will be used to correlate with the saga
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="eventProperty"></param>
        protected void MapCorrelationIdFrom<E>(Expression<Func<E, object>> eventProperty) where E : IEvent
        {
            _maps[typeof (E)] = eventProperty.GetPropertyInfo() as PropertyInfo;            
        }

      
    }
}