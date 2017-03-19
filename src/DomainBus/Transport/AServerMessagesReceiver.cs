﻿using System;
using System.Linq;
using CavemanTools;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Client;

namespace DomainBus.Transport
{
    public abstract class AServerMessagesReceiver:IReceiveServerMessages,IDisposable
    {
        /// <summary>
        /// Timer used to implement polling. Default is every 30 seconds
        /// </summary>
        protected readonly ITimer Timer;
        private IDispatchReceivedMessages _dispatcher;


        /// <summary>
        /// Uses <see cref="System.Threading.Timer"/>
        /// </summary>
        protected AServerMessagesReceiver():this(new DefaultTimer())
        {
            
        }

        protected AServerMessagesReceiver(ITimer timer)
        {
            timer.MustNotBeNull();
            Timer = timer;
            Timer.SetHandler(Callback);
            Timer.Interval = 30.ToSeconds();
        }


        protected void Callback(object state)
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
            Timer.Start();
        }

        public void Dispose()
        {
            Timer.Dispose();
        }
    }
}