using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CavemanTools.Logging;
using DomainBus.Abstractions;

namespace DomainBus.Dispatcher.Client
{
    public class MessageDispatcher:IDispatchMessages
    {
        private readonly Func<IMessage[], Task> _dispatch;
        private readonly IStoreReservedMessagesIds _reservedIds;

        public MessageDispatcher(Func<IMessage[],Task> dispatch, IStoreReservedMessagesIds reservedIds)
        {
            _dispatch = dispatch;
            _reservedIds = reservedIds;
        }

        public Task SendAsync(params ICommand[] commands)
        {
            if (commands.Length == 0) return Task.WhenAll();
            return _dispatch(commands);
        }

        public void Publish(params IEvent[] events)
        {
            if (events.Length == 0) return;
            if (events.Any(e => e.OperationId == null))
            {
                this.LogError("All events must have OperationId set. You can use the Enrol method of a command to assign the SourceId for any event resulted from the command's handling.");
                throw new InvalidOperationException("All events must have OperationId set. You can use the Enrol method of a command to assign the operation id for any event resulted from the command's handling.");
            }
            _dispatch(events).Wait();
        }

        /// <exception cref="InvalidReservationCountException"></exception>
        public Guid[] ReserveIdsFor<T>(T handlerType, Guid msgId, int howMany)
        {

            this.LogDebug("Getting reserved ids for event {0} handled by {1}", msgId, handlerType.GetType().Name);
            var input = new ReservedIdsSource() { Count = howMany, HandlerType = typeof(T), MessageId = msgId };
            var guids = _reservedIds.Get(input);
            if (guids.IsNullOrEmpty())
            {
                this.LogDebug("No ids are reserved for event {0} handled by {1}. Generating {2} ids", msgId, handlerType.GetType().Name, howMany);
                guids = Enumerable.Range(1, howMany).Select(d => Guid.NewGuid()).ToArray();
                _reservedIds.Add(input, guids);
            }
            else
            {
                if (guids.Length != howMany) throw new InvalidReservationCountException(msgId, typeof(T), howMany, guids.Length);
            }
            return guids;
        }

        public void Send(params ICommand[] commands)
        {
            SendAsync(commands).Wait();
        }

    }
}