using System;
using System.Collections.Generic;
using DomainBus.Processing;

namespace DomainBus
{
    public abstract class ASagaState : ISagaState
    {
        
        public IDictionary<string,object> RawData { get; }=new Dictionary<string, object>();


        public Guid Id { get; private set; } = Guid.NewGuid();
        public bool IsCompleted { get;  set; }
        public long AutoTimestamp { get; set; }
    }

   
}