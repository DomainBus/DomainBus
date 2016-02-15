using System;

namespace DomainBus.Audit
{
    public class MessageProcessingHandlingAudit
    {
        public string HandlerType { get; set; }
        public DateTimeOffset StartedAt { get; set; }=DateTimeOffset.Now;
        public DateTimeOffset? EndedAt { get; set; }
        public bool WasSuccessful => Error == null;
        public string Error { get; set; }
    }
}