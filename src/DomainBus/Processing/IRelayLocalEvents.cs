using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace DomainBus.Processing
{
    public interface IRelayLocalEvents
    {
        void Queue(IEvent ev);
    }

    public class RelayLocalEvents : IRelayLocalEvents
    {
        private readonly Action<IEvent> _action;

        public RelayLocalEvents(Action<IEvent> action)
        {
            _action = action;
        }
        public void Queue(IEvent ev)
        {
            if (ev==null || !ev.GetType().HasCustomAttribute<RelayLocallyAttribute>()) return;
            AddAndExecute(ev);
        }
        public Queue<IEvent> _queue=new Queue<IEvent>();
        private Task _task;
        private object _sync=new object();
        async void AddAndExecute(IEvent ev)
        {
            lock (_sync)
            {
                _queue.Enqueue(ev);
            }
            if (_task != null) return;
            _task = Task.Run(() =>
            {
                while (true)
                {
                    lock (_sync)
                    {
                        if (_queue.Count == 0) break;
                        ev = _queue.Dequeue();
                    }
                    _action(ev);
                }
                    
            });

            await _task.ConfigureFalse();
            _task = null;
        }

    }
}