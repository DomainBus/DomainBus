using System;
using System.Threading.Tasks;
using DomainBus.Dispatcher;

namespace DomainBus
{
    public interface IDispatchMessages
    {
        /// <summary>
        /// </summary>
        /// <param name="commands"></param>
        void Send(params ICommand[] commands);
        Task SendAsync(params ICommand[] commands);

        /// <summary>
        /// Publishes the events
        /// </summary>
        /// <param name="events"></param>
        void Publish(params IEvent[] events);


        ///<summary>
        /// Use it to reserve commands ids when you need to send one from an event handler.
        /// This way, you'll get the same id for the commands which helps to prevent duplicate commands
        /// </summary>
        /// <param name="handlerType">The type invoking the reservation</param>
        /// <param name="msgId">Event id </param>
        /// <param name="howMany">How many ids should be reserved</param>
        /// <exception cref="InvalidReservationCountException"></exception>
        /// <returns></returns>
        Guid[] ReserveIdsFor<T>(T handlerType, Guid msgId, int howMany);

    }
}