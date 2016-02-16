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