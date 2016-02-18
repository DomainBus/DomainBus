using System;
using System.Collections.Generic;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Client;
using DomainBus.Dispatcher.Server;

namespace DomainBus.Transport
{
    public abstract class AEndpointConfigurationReceiver : AbstractPoller, IGetEndpointUpdates
    {
        private IWantEndpointUpdates _server;

        public void Subscribe(IWantEndpointUpdates server)
        {
            _server = server;          
        }

        protected override void Callback(object state)
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