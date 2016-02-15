using DomainBus;
using DomainBus.Configuration.Internals;
using FluentAssertions;
using Xunit;

namespace Tests.Configuration
{


    public class MethodBasedDiscoveryTests
    {
        private MethodBasedDiscovery _sut;

        class MyEventHandler
        {
            [StartsSaga]
            public void Handle(MyEvent ev) { }
            
        }
        class MyCommandHandler
        {
            public void Execute(MyCommand ev) { }
            
        }

        class MixedHandler
        {
            public void Handle(MyEvent ev) { }
            public void Execute(MyCommand ev) { }
        }
        public void Handle(MyEvent ev) { }

        class NoHandlers
        {
            public int Handle(MyEvent ev) => 1;
            public void Handle(MyEvent ev,string other) { }
            public void Handle(MyCommand ev) { }
        }

        public MethodBasedDiscoveryTests()
        {
            _sut=new MethodBasedDiscovery();
        }

        [Fact]
        public void type_is_handler()
        {
            _sut.IsHandler(typeof (MyEventHandler)).Should().BeTrue();
            _sut.IsHandler(typeof (MyCommandHandler)).Should().BeTrue();
        }


        [Fact]
        public void type_is_not_handler()
        {
            _sut.IsHandler(typeof (NoHandlers)).Should().BeFalse();
        }

        [Fact]
        public void get_message_type_from_handler()
        {
            _sut.GetHandledMessageTypes(typeof(MixedHandler)).ShouldAllBeEquivalentTo(new[]{typeof(MyEvent),typeof(MyCommand)});
        }

        [Fact]
        public void detect_saga_starter()
        {
            _sut.CanStartSaga(typeof (MyEventHandler), typeof (MyEvent)).Should().BeTrue();
            _sut.CanStartSaga(typeof (MyEventHandler), typeof (MyCommand)).Should().BeFalse();
        }
    }
}