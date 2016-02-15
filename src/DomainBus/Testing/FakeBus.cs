using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainBus.Abstractions;

namespace DomainBus.Testing
{
    public class FakeBus : IDispatchMessages
    {
        private List<IMessage> _messages=new List<IMessage>();

        public List<IMessage> Messages
        {
            get { return _messages; }
        }

       
        public Guid[] ReserveIdsFor<T>(T handlerType, Guid msgId, int howMany)
        {
            return (Enumerable.Repeat<int>(1, howMany).Select(d => Guid.NewGuid()).ToArray());
        }

        public void Send(params ICommand[] commands)
        {
            Messages.AddRange(commands);
        }

        public Task SendAsync(params ICommand[] commands)
        {
            Messages.AddRange(commands);
            return Task.FromResult(false);
        }

        public void Publish(params IEvent[] events)
        {
            Messages.AddRange(events);
        }

        public IEnumerable<T> GetCommands<T>() where T : ICommand
        {
            return Messages.OfType<T>();
        }
        
        public IEnumerable<T> GetEvents<T>() where T : IEvent
        {
            return Messages.OfType<T>();
        }

    }
}