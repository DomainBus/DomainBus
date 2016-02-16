using System;
using System.Threading;

namespace DomainBus.Transport
{
    public abstract class AbstractPoller:IDisposable
    {
        private Timer _timer;

        protected AbstractPoller()
        {
            _timer = new Timer(Callback, null, -1.ToMiliseconds(), PollingInterval);
        }

        protected abstract void Callback(object state);


        public void Start()
        {
            _timer.Change(0.ToSeconds(), PollingInterval);
        }

        public void Stop()
        {
            _timer.Change(-1, Timeout.Infinite);
        }

        public TimeSpan PollingInterval { get; set; }
        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }

    }
}