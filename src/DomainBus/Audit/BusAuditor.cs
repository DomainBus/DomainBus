﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CavemanTools.Logging;
using DomainBus.Abstractions;
using DomainBus.Configuration;
using DomainBus.Processing;

namespace DomainBus.Audit
{
    public class BusAuditor
    {
        
      
        private readonly IStoreAudits _store;

        ConcurrentDictionary<string, object> _items=new ConcurrentDictionary<string, object>();

        const string LogName = "DomainBus";

        public BusAuditor(IStoreAudits store)
        {
            
           
            _store = store;
        }


        T Get<T>(IMessage msg, string processor="") where T : class
        {
            object item=null;
            _items.TryGetValue(msg.Id+processor, out item);
            return item as T;
        }

        public void StartedProcessing(string processor, IMessage message)
        {
            LogName.LogDebug($"Processor '{processor}' started to process {message}");
            if(!_items.TryAdd(message.Id+processor,new MessageProcessingAudit(message.Id))) LogName.LogWarn($"Couldn't add message process audit for {message}");
        }

        public void Handling(IMessage message, Type handlerType, string processor)
        {
            LogName.LogDebug($"({processor})Handling {message} with {handlerType}");
            Get<MessageProcessingAudit>(message,processor)
                ?.Handlers
                .Add(new MessageProcessingHandlingAudit()
                {
                    HandlerType = handlerType.FullName                    
                });

        }

        public void Handled(IMessage message, Type handlerType, string processor, Exception ex = null)
        {
            LogName.LogDebug($"({processor})Handled {message} with {handlerType}");
            var item = Get<MessageProcessingAudit>(message,processor)?.Handlers.Find(h => h.HandlerType == handlerType.FullName);
            if (item == null)
            {
                LogName.LogWarn($"Message was handled but there is no audit item available. Bug in auditor?");
                return;
            }
            item.EndedAt=DateTimeOffset.Now;
            if (ex != null)
            {
                item.Error = ex.ToString();
                LogName.LogError(ex);
            }            
        }

        public void MessageProcessed(string processor, IMessage message)
        {
            LogName.LogDebug($"({processor}) Ended processing of {message}");
            var item = Get<MessageProcessingAudit>(message,processor);
            if (item == null)
            {
                LogName.LogWarn("Audit item not found. Bug?");
                return;
            }
            item.CompletedAt=DateTimeOffset.Now;
            _store.Append(item);
            RemoveAudit(message);
        }

        void RemoveAudit(IMessage msg)
        {
            object t;
            _items.TryRemove(msg.Id.ToString(),out t);
        }
       

        #region Processor       

        public void BusConfigurationError(IMessage msg, DiContainerException ex, string name)
        {
           LogName.LogError(ex,$"({name}) Can't handle message {msg}");
            var item=Get<MessageProcessingAudit>(msg);
            if (item==null) return;
            item.ThrewConfigurationError = true;
        }

        public void BusConfigurationError(IMessage msg, MissingHandlerException ex, string name)
        {

            LogName.LogError(ex, $"({name}) Can't handle message {msg}");
            var item = Get<MessageProcessingAudit>(msg);
            if (item == null) return;
            item.ThrewConfigurationError = true;
        }

       

        #endregion

        #region Host

        public void MessagesRejected(EndpointId destination, IEnumerable<IMessage> msg)
        {
            _store.Append(msg.Select(m=>new MessageRejectedAudit(m.Id,destination)).ToArray());
        }

        public void DispatchEnded(IMessage[] toSend)
        {
            _store.Append(toSend.Select(m =>Get<MessageTransportAudit>(m))
                .FilterNulls()
                .ToArray());
            toSend.ForEach(RemoveAudit);
        }

        public void DispatchStarted(string hostName, IEnumerable<IMessage> commit)
        {
            commit.ForEach(m => _items.TryAdd(m.Id.ToString(), new MessageTransportAudit(m.Id) {ByHost = hostName}));
        }

        public void SentToServer(IEnumerable<IMessage> commit)
        {
            commit.Select(m =>
            {
                LogName.LogDebug($"Message {m} sent to server");
                return Get<MessageTransportAudit>(m);
            }).FilterNulls().ForEach(fe => fe.Destinations.Add(new MessageTransportAudit.DestinationRecord(MessageTransportAudit.ServerEndpoint, DateTimeOffset.Now)));
        }

        public void SentToLocal(string processor, IEnumerable<IMessage> commit)
        {
            commit.Select(m =>
            {
                LogName.LogDebug($"Message {m} sent to local processor {processor}");
                return Get<MessageTransportAudit>(m);
            }).FilterNulls()
            .ForEach(fe=>fe.Destinations.Add(new MessageTransportAudit.DestinationRecord(processor,DateTimeOffset.Now)));
            
        }

        public void AddedToProcessingStorage(string processor, IEnumerable<IMessage> commit)
        {
            _store.Append(commit.Select(m =>
            {
                LogName.LogDebug($"({processor}) Message {m} added to storage");
                return new StoredMessageAudit()
                {
                    MessageId = m.Id,
                    Processor = processor
                };
            }).ToArray());
        }

        internal void BusConfigurationError(IMessage msg, SagaConfigurationException ex, string name)
        {
            LogName.LogError(ex, $"({name}) Can't handle message {msg}");
            var item = Get<MessageProcessingAudit>(msg);
            if (item == null) return;
            item.ThrewConfigurationError = true;
        }

        #endregion


    }
}