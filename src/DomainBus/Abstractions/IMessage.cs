using System;

namespace DomainBus.Abstractions
{
    public interface IMessage
    {
        Guid Id { get; }
        DateTimeOffset TimeStamp { get; }      

        /// <summary>
        /// For idempotency purposes. By default, the value for a command is the command id. 
        /// For events the value should be the command id whose handling generated the event
        /// </summary>
        Guid? OperationId { get; set; }
                
    }
}