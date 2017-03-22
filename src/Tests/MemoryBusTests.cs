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

        public class TestContainerBuilder : IRegisterBusTypesInContainer
        {
            public static TestContainerBuilder Instance= new TestContainerBuilder();

            public TestContainerBuilder()
            {
                
            }
                    
            public void RegisterSingletonInstance<T>(T instance)
            {
                
            }

            public void Register(Type[] types, bool asSingleton = false)
            {
                
            }

            public void RegisterInstanceFactory<T>(Func<T> instance)
            {
                
            }
        }

        [Fact]
        public void using_memory_bus()
        {
            ServiceBus.ConfigureAsMemoryBus(TestContainerBuilder.Instance, typeof(SomeHandler));
            var bus = ServiceBus.Build(ActivatorContainer.Instance);
            var sut = bus.GetDispatcher();
            var command = new MyCommand();
            sut.Send(command);
            bus.GetProcessingQueue(ServiceBus.MemoryProcessor).WaitUntilWorkersFinish();
            _results.Contains(typeof(MyCommand)).Should().BeTrue();
            sut.Publish(new MyEvent().EnrolInOperation(command));
            bus.GetProcessingQueue(ServiceBus.MemoryProcessor).WaitUntilWorkersFinish();
            _results.Contains(typeof(MyEvent)).Should().BeTrue();

        }

    }
}