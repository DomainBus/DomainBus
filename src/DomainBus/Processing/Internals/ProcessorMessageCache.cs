using System;
using System.Collections.Generic;
using System.Linq;
using CavemanTools.Logging;
using DomainBus.Abstractions;

namespace DomainBus.Processing.Internals
{
    public class ProcessorMessageCache
    {
       
        List<IMessage> _cache=new List<IMessage>();

        HashSet<Guid> _set=new HashSet<Guid>();

        object _sync=new object();
     
     
        public IEnumerable<IMessage> Cache => _cache;


        public void Add(IEnumerable<IMessage> msgs)
        {
          
            lock (_sync)
            {
                 _cache.AddRange(msgs.Where(m => _set.Add(m.Id)));                               
            }                        
        }

        /// <summary>
        /// Gets a message or null if none available
        /// </summary>
        /// <returns></returns>
        public IMessage GetNextMessage()
        {
            lock (_sync)
            {
                return _cache.OrderBy(d => d.TimeStamp).FirstOrDefault();
            }            
        }

      
        public void Remove(IMessage msg)
        {
           lock (_sync)
           {
                _cache.Remove(msg);
                if (_set.Remove(msg.Id))
                {
                    this.LogDebug($"Message with id {msg} removed from cache");
                }
                else
                {
                    this.LogDebug($"Nothing to remove. Message with id {msg} doesn't exist in cache");
                }
            }
            
        }

       
    }
}