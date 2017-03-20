using System;
using System.Linq;
using System.Threading.Tasks;
using CavemanTools;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Server;

namespace DomainBus.Transport
{
    public abstract class AClientMessagesReceiver : IDisposable, IGetMessages
    {
        protected readonly ITimer Timer;
        private IRouteMessages _server;

        protected AClientMessagesReceiver():this(new DefaultTimer())
        {
            
        }

        protected AClientMessagesReceiver(ITimer timer)
        {
            Timer = timer;
            Timer.Interval = 30.ToSeconds();
            Timer.SetHandler(Callback);
        }

        public void Subscribe(IRouteMessages router)
        {
            _server = router;
        }

        protected void Callback(object state)
        {
            var items = GetMessages();

            items.ForEach(async d =>
            {
                try
                {
                    await _server.Route(d).ConfigureFalse();
                    MarkAsHandled(d);                    
                }
                catch (Exception ex)
                {
                    this.LogError(ex);
                }
            });
            
            
        }

        protected abstract EnvelopeFromClient[] GetMessages();
        protected abstract void MarkAsHandled(EnvelopeFromClient envs);
        public void Dispose()
        {
            Timer.Dispose();
        }

        public void Start()
        {
           Timer.Start();
        }
    }
}