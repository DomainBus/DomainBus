using System;
using DomainBus.Processing;
using DomainBus.Processing.Internals;
using NSubstitute;
using Xunit;

namespace Tests.Processing.Saga
{
    public class SagaStarterExecutorTests:BaseSagaTests
    {
        Guid _entityId = Guid.NewGuid();
        private SagaStarterExecutor _sut;

        public SagaStarterExecutorTests()
        {
            _sut = new SagaStarterExecutor(_invoker, _di);
        }

        [Fact]
        public void start_saga_normally()
        {
            _sut.Handle(new StartEvent() {EntityId = _entityId},"");
            _storage.Received(1).Save(_saga,Arg.Any<string>(),true);
        }

        [Fact]
        public void if_saga_exists_nothing_happens()
        {
            _storage.WhenForAnyArgs(d=>d.Save(null,null,false)).Throw(new SagaExistsException());
            _sut.Handle(new StartEvent() { EntityId = _entityId }, "");
            _storage.ReceivedWithAnyArgs(1).Save(null,null,false);
        }

    }
}