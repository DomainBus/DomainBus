using System;
using System.Collections.Generic;

namespace DomainBus.Audit
{
    public class MessageTransportAudit:IAuditMessageId
    {
        public const string ServerEndpoint = "server";

        public struct DestinationRecord
        {
            public string Endpoint;
            public DateTimeOffset DeliveredAt;

            public DestinationRecord(string endpoint, DateTimeOffset deliveredAt)
            {
                Endpoint = endpoint;
                DeliveredAt = deliveredAt;
            }
        }

        public Guid MessageId { get; private set; }
        public DateTimeOffset DispatchedAt { get; set; }=DateTimeOffset.Now;
        public string ByHost { get; set; }
        
        public List<DestinationRecord> Destinations { get;  }=new List<DestinationRecord>();



        public MessageTransportAudit(Guid messageId)
        {
            MessageId = messageId;
        }


    }
}