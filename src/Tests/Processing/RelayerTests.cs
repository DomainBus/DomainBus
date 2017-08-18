 
using FluentAssertions;
using Xunit;
using System;
using System.Threading.Tasks;
using CavemanTools.Infrastructure;
using DomainBus;
using DomainBus.Configuration;
using DomainBus.Processing;


namespace Tests.Processing
{
    public class RelayerTests
    {
        private IDomainBus _bus;

        [RelayLocally]
        public class SomeEvent:AbstractEvent
        {
            public SomeEvent()
            {
                OperationId=Guid.NewGuid();
            }
        }
      public class SomeOtherEvent:AbstractEvent
        {
            public SomeOtherEvent()
            {
                OperationId=Guid.NewGuid();
            }
        }

        

        [DomainBus(ServiceBus.EventsProcessor)]
        public class Handler
        {
            public void Handle(SomeEvent ev)
            {
                
            }

            public void Handle(SomeOtherEvent ev)
            {
                
            }

            public void Execute(RelayCommand cmd)
            {
                
            }
        }

        private IEvent _last = null;
        private int _count;

        public RelayerTests()
        {
            _bus = ServiceBus.ConfigureForMonolith(c =>
            {
                c.AutoConfigureFrom(new []{typeof(Handler)})
                .RegisterTypesInContainer(new MemoryBusTests.TestContainerBuilder())
                .DefaultProcessors().RelayEventsLocally(cf => cf.Send(Relay));
            }).Build(new DependencyContainerWrapper(t=>t.CreateInstance()));

            

        }

        void Relay(IEvent ev)
        {
            _last = ev;
            _count++;
        }

        [Fact]
        public async Task simple_relay()
        {
            var relay = new RelayLocalEvents(Relay);
            relay.Queue(new SomeEvent());
            await Task.Delay(100);
            _last.Should().NotBeNull();
            _count.Should().Be(1);
        }

        [Fact]
        public async Task bus_relay_marked_event()
        {
            _bus.GetDispatcher().Publish(new SomeEvent());
            await Task.Delay(100);
            _last.Should().NotBeNull();
            _last.Should().BeOfType<SomeEvent>();
            _count.Should().Be(1);
        }

        [Fact]
        public async Task bus_dont_relay_unmarked_event()
        {
            _bus.GetDispatcher().Publish(new SomeOtherEvent());
            await Task.Delay(100);
            _last.Should().BeNull();
            _count.Should().Be(0);
        }

        [RelayLocally]
        public class RelayCommand : AbstractCommand
        {
            
        }

        [Fact]
        public async Task bus_dont_relay_commands()
        {
            await _bus.GetDispatcher().SendAsync(new RelayCommand());
            await Task.Delay(100);
            _last.Should().BeNull();
            _count.Should().Be(0);
        }


    }
} 
