using System;
using System.Collections.Generic;
using CavemanTools;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Client;
using DomainBus.Dispatcher.Server;

namespace DomainBus.Transport
{
    public abstract class AEndpointConfigurationReceiver : IDisposable, IGetEndpointUpdates
    {
        protected readonly ITimer Timer;
        private IWantEndpointUpdates _server;

        protected AEndpointConfigurationReceiver():this(new DefaultTimer())
        {
            
        }

        protected AEndpointConfigurationReceiver(ITimer timer)
        {
            timer.MustNotBeNull();
            Timer = timer;
            Timer.Interval = 30.ToSeconds();
            Timer.SetHandler(Callback);
        }

        public void Dispose()
        {
            Timer.Dispose();
        }

        public void Start()
        {
            Timer.Start();
        }

        public void Subscribe(IWantEndpointUpdates server)
        {
            _server = server;          
        }

        protected void Callback(object state)
        {
            try
            {
                var items = GetConfigs();
                _server.ReceiveConfigurations(items);
                MarkAsHandled(items);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }

        protected abstract EndpointMessagesConfig[] GetConfigs();
        protected abstract void MarkAsHandled(IEnumerable<EndpointMessagesConfig> configs);
    }
}