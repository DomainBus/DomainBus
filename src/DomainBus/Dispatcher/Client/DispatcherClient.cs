using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CavemanTools.Logging;
using DomainBus.Abstractions;
using DomainBus.Audit;
using DomainBus.Configuration;
using DomainBus.Transport;

namespace DomainBus.Dispatcher.Client
{
    public class DispatcherClient:IDispatcherClient, IDispatchReceivedMessages
    {
        private readonly string _hostName;
        private readonly ITalkToServer _server;
        private readonly IDeliveryErrorsQueue _errors;
        private readonly BusAuditor _auditor;


        public DispatcherClient(string hostName,ITalkToServer server, IDeliveryErrorsQueue errors ,BusAuditor auditor)
        {
            server.MustNotBeNull();
            _hostName = hostName;
            _server = server;
            _errors = errors;
            _auditor = auditor;
        }

        public async Task Dispatch(params IMessage[] messages)
        {
            if (messages.Length == 0) return;

            _auditor.DispatchStarted(_hostName, messages);
            var local = SendLocally(messages);

            var toRelay =
                messages.Where(m => !m.GetType().GetTypeInfo().HasCustomAttribute<LocalOnlyAttribute>()).ToArray();

            var remote = SendToServer(toRelay);

            await Task.WhenAll(local, remote).ConfigureFalse();


            _auditor.DispatchEnded(messages);


        }

        private async Task SendToServer(IMessage[] toRelay)
        {
            if (toRelay.Length == 0) return;
            try
            {
                await _server.SendMessages(new EnvelopeFrom()
                {
                    From = _hostName,Messages = toRelay
                }).ConfigureFalse();
                _auditor.SentToServer(toRelay);
            }
            catch (CouldntSendMessagesException ex)
            {
                this.LogError(ex);
                _errors.TransporterError(ex);
            }
            
        }

        async Task SendLocally(IMessage[] messages)
        {

            foreach (var config in _endpoints)
            {
                var all = config.Filter(messages);
                if (all.Length > 0)
                {
                    await SendToProcessor(config, all);                   
                }
            }
        }

        List<IEndpointConfiguration> _endpoints=new List<IEndpointConfiguration>();


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="EndpointNotFoundException"></exception>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task DeliverToLocalProcessors(EnvelopeTo envelope)
        {
            if (envelope.Messages.Length == 0)
            {
                this.LogWarn("Received an empty envelope from server");
                return;
            }
           var ep = _endpoints.FirstOrDefault(d => d.Id==envelope.To);
            if (ep == null)
            {
               _errors.UnknownEndpoint(new EndpointNotFoundException(envelope.To, _hostName));
                return;
            }

            var toSend = ep.Filter(envelope.Messages);

            if (toSend.Length != envelope.Messages.Length)
            {
                var diff = toSend.Compare(envelope.Messages, (m1, m2) => m1.Id == m2.Id);
                _errors.MessagesRejected(ep.Id, diff.Removed.ToArray()); 
                _auditor.MessagesRejected(envelope.To,diff.Removed);
            }
            _auditor.DispatchStarted(_hostName,toSend);
            await SendToProcessor(ep, toSend);
            _auditor.DispatchEnded(toSend);
        }

        private async Task SendToProcessor(IEndpointConfiguration ep, IMessage[] toSend)
        {
            this.LogDebug($"Sending {toSend.Length} messages to {ep.Id}");
            await ep.AddToProcessing(toSend);
            _auditor.SentToLocal(ep.Id, toSend);
        }

     

        public void SubscribeToServer(IEnumerable<IEndpointConfiguration> configs)
        {
            _endpoints.Clear();
            _endpoints.AddRange(configs);
            
            var serverUpdate = _endpoints.Select(d => new EndpointMessagesConfig()
            {
                Endpoint = d.Id,
                MessageTypes = d.HandledMessagesTypes.Select(m => m.AsMessageName()).ToArray()
            });
            _server.SendEndpointsConfiguration(serverUpdate);

            this.LogInfo($"Subscribed endpoints [{_endpoints.Select(d => d.Id).StringJoin()}]");
        }
    }
}