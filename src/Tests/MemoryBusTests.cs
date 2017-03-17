using System;
using System.Collections.Generic;
using System.Diagnostics;
using CavemanTools.Infrastructure;
using CavemanTools.Logging;
using DomainBus;
using DomainBus.Configuration;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class MemoryBusTests
    {
        static List<Type> _results=new List<Type>();
        public MemoryBusTests()
        {
            
        }

        public class SomeHandler
        {
            public void Handle(MyEvent ev)
            {
                _results.Add(ev.GetType());
            }

            public void Execute(MyCommand cmd)
            {
                _results.Add(cmd.GetType());
            }
        }

        public class TEstBuilder : AbstractBusBuilder
        {
            public TEstBuilder()
            {
                
            }
            protected override void RegisterSingletonInstance<T>(T instance)
            {
                
            }

            protected override void Register(IEnumerable<Type> types,bool sginle=false)
            {
                
            }

            protected override void RegisterInstanceFactory<T>(Func<T> instance)
            {
                
            }

            protected override void ContainerConfigurationCompleted()
            {
                
            }

            protected override IContainerScope BuildContainerScope()
            {
                return ActivatorContainer.Instance;
            }
        }

        [Fact]
        public void using_memory_bus()
        {
            LogManager.OutputTo(s=>Debug.WriteLine(s));
            var bus = ServiceBus.ConfigureWith(new TEstBuilder()).AsMemoryBus(typeof (SomeHandler));
            var sut = bus.GetDispatcher();
            var command = new MyCommand();
            sut.Send(command);
            bus.GetProcessingQueue("memorybus").WaitUntilWorkersFinish();
            _results.Contains(typeof(MyCommand)).Should().BeTrue();
            sut.Publish(new MyEvent().EnrolInOperation(command));
            bus.GetProcessingQueue("memorybus").WaitUntilWorkersFinish();
            _results.Contains(typeof(MyEvent)).Should().BeTrue();

        }

    }
}