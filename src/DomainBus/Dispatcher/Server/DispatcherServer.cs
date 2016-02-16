using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Client;
using DomainBus.Transport;

namespace DomainBus.Dispatcher.Server
{
    public class DispatcherServer:IWantEndpointUpdates,IRouteMessages
    {
        private readonly DispatchServerConfiguration _config;
        
        private DispatcherState _state=new DispatcherState();
        
        public DispatcherServer(Action<DispatchServerConfiguration> configAction)
        {
            var config=new DispatchServerConfiguration();
            configAction(config);
            config.Validate();
            _config = config;   
            _config.EndpointUpdatesNotifier.Subscribe(this);
            _config.MessageNotifier.Subscribe(this);              
        }


        public void LoadState()
        {
           this.LogInfo("Loading state");
            var state = _config.Storage.Load();
            if (state != null)
            {
                _state = state;
            }

            this.LogInfo("State loaded");
        }

        public void ReceiveConfigurations(IEnumerable<EndpointMessagesConfig> update)
        {
            _state.Update(update);
            _config.Storage.Save(_state);
            
        }
    

        public Task Route(EnvelopeFromClient envelope)
        {
            var delivery = _state.GetEnvelopes(envelope)
                .Select(d =>
                {
                    var transporter = _config.Transporters.GetTransporter(d.To);
                    if (transporter == null)
                    {
                        this.LogError($"There is no transporter defined for {d.To}");
                        return TasksUtils.EmptyTask();
                    }
                    return Send(transporter,d);
                }).ToArray();

           return Task.WhenAll(delivery);
        }

        private async Task Send(IDeliverToEndpoint transporter, EnvelopeToClient envelopeTo)
        {
            try
            {
                await transporter.Send(envelopeTo).ConfigureFalse();
            }
            catch (CouldntSendMessagesException ex)
            {
                this.LogError(ex);
                _config.DeliveryErrorsQueue.TransporterError(ex);
            }
            //we need the server to not crush if one transporter throws an unhandled exception
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }

     
    }
}