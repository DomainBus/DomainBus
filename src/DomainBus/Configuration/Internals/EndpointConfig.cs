using System;
using System.Threading.Tasks;
using CavemanTools.Logging;
using DomainBus.Abstractions;
using DomainBus.Dispatcher.Client;
using DomainBus.Processing.Internals;

namespace DomainBus.Configuration.Internals
{
    internal class EndpointConfig:IEndpointConfiguration
    {
        private readonly ProcessingService _queue;

        public EndpointConfig(ProcessingService queue)
        {
            _queue = queue;        
        }

        public EndpointId Id { get;  set; }

        public Type[] HandledMessagesTypes { get; set; }
        
        public ProcessingService Processor => _queue;

        public Task AddToProcessing(params IMessage[] messages)
        {
            this.LogDebug($"Adding {messages.Length} messages to local processor: {_queue.Name}");
            return _queue.Queue(messages);            
        }
    }
}