using System.Collections.Generic;
using System.Linq;
using CavemanTools.Logging;
using DomainBus.Abstractions;
using DomainBus.Configuration;
using DomainBus.Dispatcher.Client;
using DomainBus.Transport;

namespace DomainBus.Dispatcher.Server
{
    public class DispatcherState
    {
        object _sync=new object();

       Dictionary<EndpointId,string[]> _items=new Dictionary<EndpointId, string[]>();

        public KeyValuePair<EndpointId, string[]>[] Items
        {
            get
            {
                lock (_sync)
                {
                    return _items.ToArray();
                }
            }
            private set
            {
                lock (_sync)
                {
                    _items = value.ToDictionary(d => d.Key, d => d.Value);
                }
          
            }
        }

        public void Update(IEnumerable<EndpointMessagesConfig> update)
        {
            lock (_sync)
            {
             update.ForEach(u =>
             {
                 _items[u.Endpoint] = u.MessageTypes.ToArray();
                 this.LogInfo($"State updated for endpoint {u.Endpoint}");
             });                                     
            }
        }

      
        IEnumerable<IMessage> FilterEndpointMessages(string[] msgTypes, IEnumerable<IMessage> messages)
            => messages.Where(m => msgTypes.Contains(m.GetType().AsMessageName()));
        
       
        public EnvelopeToClient[] GetEnvelopes(EnvelopeFromClient envelope)
        {
            IEnumerable<KeyValuePair<EndpointId, string[]>> items= Items;
          
            return items
                    .Where(d => d.Key.Host != envelope.From)
                    .Select(kv =>
                        new EnvelopeToClient()
                        {
                            To = kv.Key,
                            Messages = FilterEndpointMessages(kv.Value, envelope.Messages).ToArray()
                        })
                    .Where(e => e.Messages.Any())
                    .ToArray();

        }
        
      
    }
}