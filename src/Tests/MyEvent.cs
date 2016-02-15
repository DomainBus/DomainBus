using System;
using DomainBus;
using DomainBus.DomainEvents;

namespace Tests
{
    public class MyEvent:DomainEvent
    {
         
    }

    public class MyCommand : AbstractCommand
    {
        public Guid SomeId { get; set; }
    }
}