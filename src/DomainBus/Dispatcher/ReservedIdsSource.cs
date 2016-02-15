using System;

namespace DomainBus.Dispatcher
{
    public class ReservedIdsSource
    {
        public Type HandlerType { get; set; }
        public Guid MessageId { get; set; }
        public int Count { get; set; }
    }
}