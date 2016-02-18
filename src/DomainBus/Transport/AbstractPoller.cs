using System;
using System.Threading;

namespace DomainBus.Transport
{
    public abstract class AbstractPoller:IDisposable
    {
        private Timer _timer;

        protected AbstractPoller()
        {
            //_timer = new Timer(Callback, null, -1.ToMiliseconds(), PollingInterval);            
        }

        protected abstract void Callback(object state);


        public void Start()
        {
            Stop();
            _timer = new Timer(Callback, null, 0.ToMiliseconds(), PollingInterval);
        }

        public void Stop()
        {
            _timer?.Dispose();
        }

        public TimeSpan PollingInterval { get; set; } = 30.ToSeconds();
        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }

    }
}