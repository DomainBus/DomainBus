using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainBus.Audit
{
    /// <summary>
    /// Sohuld be used ONLY in development for debugging purposes
    /// </summary>
    public class InMemoryAuditStorage : IStoreAudits
    {
        public Dictionary<Guid, List<object>> Data { get; } = new Dictionary<Guid,List<object>>();

        object _sync=new object();

        public void Append(params MessageTransportAudit[] audits)
        {
            Add(audits);
        }

        private void Add<T>(params T[] audits) where T:IAuditMessageId
        {
            lock (_sync)
            {
                audits.ForEach(a =>
                {
                    var l = Data.GetValueOrCreate(a.MessageId, () => new List<object>());
                    l.Add(a);
                });
            }
        }

        public void Append(params MessageRejectedAudit[] audits)
        {
            Add(audits);
        }

        public void Append(MessageProcessingAudit audits)
        {
            Add(audits);
        }

        public void Append(params StoredMessageAudit[] audits)
        {
            Add(audits);
        }
    }
}