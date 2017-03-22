 
using FluentAssertions;
using Xunit;
using System;
using System.Linq;
using DomainBus;
using DomainBus.Configuration;
using NSubstitute;


namespace Tests.Configuration
{
    public class MyHandler
    {
        public void Handle(MyEvent ev)
        {
            
        }
    }

    public class Integration_with_container
    {
        private IRegisterBusTypesInContainer _container = Substitute.For<IRegisterBusTypesInContainer>();
        private IBuildBus _sut;

        public Integration_with_container()
        {
            _sut= ServiceBus.ConfigureAsMemoryBus(_container, typeof(MyHandler));
        }

        [Fact]
        public void handlers_are_registered()
        {
            _container.Received().Register(Arg.Is<Type[]>(d=>d.Length==1 && d[0]==typeof(MyHandler)),false);
        }


        [Fact]
        public void bus_is_registered()
        {
            _container.Received(1).RegisterSingletonInstance(Arg.Any<IDomainBus>());
        }


        [Fact]
        public void dispatcher_factory_is_registered()
        {
            _container.Received(1).RegisterInstanceFactory(Arg.Any<Func<IDispatchMessages>>());
        }
    }
} 
