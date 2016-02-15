using System;
using System.Collections.Generic;
using System.Linq;
using DomainBus.Dispatcher;
using DomainBus.Processing;

namespace DomainBus
{
    public static class SagaExtensions
    {
        ///<summary>
        /// Use it to reserve commands ids when a saga completes and you need to send a command.
        /// This way, you'll get the same id for the commands which helps to prevent duplicate commands
        /// </summary>
        /// <exception cref="InvalidReservationCountException"></exception>
        public static Guid[] ReserveIdsForSaga<T>(this IDispatchMessages bus, T sagaState, int howMany)
            where T : ISagaState
            => bus.ReserveIdsFor(typeof (T), sagaState.Id, howMany);
        

        public static Saga<T> SagaCompletesWhen<T>(this Saga<T> saga,Func<T,bool> when) where T : ASagaState
        {
            when.MustNotBeNull();
            saga.Completion = when;
            return saga;
        }

        public static Saga<T> OnCompleteDo<T>(this Saga<T> saga, Action<T> action) where T : ISagaState
        {
            action.MustNotBeNull();
            saga.OnComplete = action;
            return saga;
        }

        //  public static void Set<T>(this ASagaState state, string key, T value) => state.RawData[key] = value;

        public static T Get<T>(this ASagaState state, string key, T defValue = default(T))
            => state.RawData.GetValue(key, defValue);

        /// <summary>
        /// True if all specified keys are present
        /// </summary>
        /// <param name="state"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Has(this ASagaState state, params string[] key)
            => key.All(k => state.RawData.ContainsKey(k));

        /// <summary>
        /// Sets a value in saga data. It also checks to see if the saga has been completed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="saga"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<T, V>(this Saga<T> saga, string key, V value) where T : ASagaState
        {
            saga.Data.RawData[key] = value;
            if (saga.Completion(saga.Data)) saga.MarkAsCompleted();
        }
    }
}