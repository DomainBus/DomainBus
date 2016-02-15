using System;

namespace DomainBus.Abstractions
{
    public abstract class BaseMessage:IMessage
    {
        protected BaseMessage()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTimeOffset.Now;
        }

        public Guid Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public Guid? OperationId { get; set; }

        public override string ToString()
        {
            return GetType().Name + " (Id: " + Id + ")";
        }
    }
}