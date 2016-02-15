using System;
using DomainBus;
using DomainBus.Processing.Internals;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Tests.Processing.Saga
{
    public class SagaExecutorTests:BaseSagaTests
    {
        private SagaExecutor _sut;

        public SagaExecutorTests()
        {
            _sut=new SagaExecutor(_invoker,_di);
            
        }

        [Fact]
        public void normal_usage()
        {
            _invoker.When(d=>d.HandleMessage(Arg.Any<MyEvent>(), _handler, "")).Do(i=>_handler.Handle(i.Arg<MyEvent>()));
            _sut.Handle(new MyEvent() {EntityId = Guid.NewGuid()},"");
            _storage.Received(1).Save(_saga,Arg.Any<string>(),false);
            _saga.RawData["ev"].Should().Be(true);
        }

        [Fact]
        public void completed_sagas_are_ignored()
        {
            _saga.IsCompleted = true;
            _sut.Handle(new MyEvent() { EntityId = Guid.NewGuid() }, "");
            _storage.DidNotReceiveWithAnyArgs().Save(_saga, Arg.Any<string>(), false);
        }

        [Fact]
        public void saga_completion()
        {
            var id = Guid.NewGuid();
            MyCommand mc=null;
             _bus.Send(Arg.Do<ICommand[]>(i => mc = i[0] as MyCommand));
            //_bus.When(d => d.Send(Arg.Any<ICommand>())).Do(i =>
            //{
            //    mc = i[0] as MyCommand;
            //});
            _invoker.When(d => d.HandleMessage(Arg.Any<IEvent>(), _handler, "")).Do(i => _handler.Handle((dynamic)i[0]));
            _sut.Handle(new StartEvent() {EntityId = id}, "");
            _sut.Handle(new MyEvent() {EntityId = id}, "");
            _sut.Handle(new OtherEvent() {EntityId = id}, "");
           

            _bus.Received(1).Send(Arg.Any<MyCommand>());
            mc.SomeId.Should().Be(id);
            mc.OperationId.Should().Be(_saga.Id);
        }
    }
}