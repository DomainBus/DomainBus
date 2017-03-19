using System;
using System.Linq;
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

            items.ForEach(d =>
            {
                try
                {
                    var t = _server.Route(d);
                    t.ContinueWith((tsk) =>
                    {
                        if (!tsk.IsFaulted)
                        {
                            MarkAsHandled(d);
                            return;
                        }
                        this.LogError(tsk.Exception);
                    });
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