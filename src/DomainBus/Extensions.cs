using System;
using System.Collections.Generic;
using System.Linq;
using DomainBus.Abstractions;

namespace DomainBus
{
    public static class Extensions
    {
        ///// <summary>
        ///// Tries to combine events of the same type and from the same origin if possible. If an event implements <see cref="ICanBeAggregated{TSelf}"/> the interface method
        ///// is used, else the most recent event is selected
        ///// </summary>
        ///// <param name="messages"></param>
        ///// <returns></returns>
        //public static IEnumerable<IMessage> AggregateEvents(this IEnumerable<IMessage> messages)
        //{
        //    if (messages.Count() <= 1)
        //    {
        //        return messages;
        //    }
        //    return messages.GroupBy(m => m.OperationId).SelectMany(mg => mg.GroupBy(m => m.GetType())
        //        .Select(grp =>
        //        {
        //            if (grp.Count() == 1)
        //            {
        //                return grp.First();
        //            }
        //            if (grp.Key.ImplementsGenericInterface(typeof(ICanBeAggregated<>), grp.Key))
        //            {
        //                var final = grp.Aggregate((m1, m2) =>
        //                {
        //                    var minfo = m1.GetType().GetMethod("Aggregate");

        //                    return (minfo.Invoke(m1, new object[] { m2 })) as IEvent;
        //                });
        //                final.OperationId = mg.Key;
        //                return final;
        //            }
        //            return grp.OrderByDescending(m => m.TimeStamp).First();
        //        }));
        //}

        

        public static T[] EnrolEventsInOperation<T>(this IEnumerable<T> list, ICommand cmd) where T : IEvent 
            => EnrolEventsInOperation(list,cmd.Id);

        public static T[] EnrolEventsInOperation<T>(this IEnumerable<T> list, Guid id) where T : IEvent 
            => list
                .Select(i =>
                    {
                        i.OperationId = id;
                        return i;
                    })
                .ToArray();

        /// <summary>
        /// Makes the event as if it was created by the command
        /// </summary>
        /// <param name="cmd"></param>
        public static T EnrolInOperation<T>(this T ev,ICommand cmd) where T:IEvent
        {
            if (cmd == null) throw new ArgumentNullException();
            cmd.Enrol(ev);
            return ev;
        }

        /// <summary>
        /// Enrols the event in the current operation.
        /// An event is enroled in an operation when it contains the operation id
        /// </summary>
        /// <param name="evnt"></param>
        public static T Enrol<T>(this T cmd,IEvent evnt) where T:ICommand
        {
            evnt.OperationId = cmd.Id;
            return cmd;
        }

        /// <summary>
        /// Creates an event enroled in the current operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static T CreateEvent<T>(this IMessage msg,Action<T> constructor = null) where T : IEvent, new()
        {
            var res = new T();
            res.OperationId = msg.Id;
            constructor?.Invoke(res);
            return res;
        }

        /// <summary>
        /// Creates a command enroled in the current operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static T CreateCommand<T>(this IMessage msg,Action<T> constructor = null) where T : ICommand, new()
        {
            var res = new T();
            res.OperationId = msg.Id;
            constructor?.Invoke(res);
            return res;
        }
    }
}