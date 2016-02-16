using System.Linq;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Server;

namespace DomainBus.Transport
{
    public abstract class AClientMessagesReceiver : AbstractPoller, IGetMessages
    {
        private IRouteMessages _server;

        public void Subscribe(IRouteMessages router)
        {
            _server = router;
        }

        protected override void Callback(object state)
        {
            var items = GetMessages();

            items.ForEach(d =>
            {
                var t = _server.Route(d);;
                t.ContinueWith((tsk) =>
                {
                    if (!tsk.IsFaulted)
                    {
                        MarkAsHandled(d);
                        return;
                    }
                    this.LogError(tsk.Exception);
                });
            });
            
            
        }

        protected abstract EnvelopeFromClient[] GetMessages();
        protected abstract void MarkAsHandled(EnvelopeFromClient envs);
    }
}