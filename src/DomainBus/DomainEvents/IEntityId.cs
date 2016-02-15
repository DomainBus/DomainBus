using System;

namespace DomainBus.DomainEvents
{
    public interface IEntityId
    {
        Guid EntityId { get; }
    }
}