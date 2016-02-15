using System;
using CavemanTools.Infrastructure;
using DomainBus;
using DomainBus.Audit;
using DomainBus.Processing;
using DomainBus.Processing.Internals;
using DomainBus.Repositories;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Tests.Processing
{
    public class HandlerTypeInvokerTests
    {
        private HandlerTypeInvoker _sut;
        private IContainerScope _di;
        private IFailedMessagesQueue _err;
        private MyHandler _handler;

        public HandlerTypeInvokerTests()
        {
            _di = Substitute.For<IContainerScope>();
            _err = Substitute.For<IFailedMessagesQueue>();
            _di.BeginLifetimeScope().Returns(_di);
            _handler=new MyHandler();
            _di.Resolve(typeof (MyHandler)).Returns(_handler);
            _sut =new HandlerTypeInvoker(typeof(MyHandler),_di,new BusAuditor(NullStorage.Instance), _err);
        }

       public class MyHandler
        {
            public bool Handled;

           public void Handle(MyEvent ev)
           {
               Handled = true;
           }

           public void Execute(MyCommand ev)
            {
                Handled = true;
            }

           public void Handle(ThrowEvent ev)
           {
               throw new Exception();
           }
        }

        public class ThrowEvent:AbstractEvent
        {
            
        }

        [Fact]
        public void normal_usage()
        {
            _sut.HandlerType.Should().Be<MyHandler>();
            _handler.Handled.Should().BeFalse();
            _sut.Handle(new MyEvent(), "");
            _handler.Handled.Should().BeTrue();

            _handler.Handled = false;
            _sut.Handle(new MyCommand(), "");
            _handler.Handled.Should().BeTrue();
        }

        [Fact]
        public void when_type_doesnt_exist_in_container_it_throws()
        {
            _di.Resolve(typeof (MyHandler)).Throws(new Exception());
            _sut.Invoking(d=>d.Handle(new MyEvent(), "")).ShouldThrow<DiContainerException>();
        }

        [Fact]
        public void when_handling_fails_it_gets_sent_to_err_queue()
        {
            var msg = new ThrowEvent();
            _sut.Handle(msg,"");
            _err.Received(1).MessageHandlingFailed(msg,Arg.Any<HandledMessageException>());
        }
    }
}