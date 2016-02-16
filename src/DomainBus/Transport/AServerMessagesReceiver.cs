using System;
using System.Linq;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Client;

namespace DomainBus.Transport
{
    public abstract class AServerMessagesReceiver:AbstractPoller,IReceiveServerMessages
    {
        private IDispatchReceivedMessages _dispatcher;

        protected override void Callback(object state)
        {
            var items = GetMessages();
            items.ForEach(env =>
            {
                try
                {
                    var t = _dispatcher.DeliverToLocalProcessors(env);
                    t.ContinueWith(tsk =>
                    {
                        if (!tsk.IsFaulted)
                        {
                            MarkAsHandled(env);
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

        protected abstract EnvelopeToClient[] GetMessages();
        protected abstract void MarkAsHandled(EnvelopeToClient env);

        public void StartReceiving(IDispatchReceivedMessages dispatcher)
        {
            _dispatcher = dispatcher;
            Start();
        }
    }
}