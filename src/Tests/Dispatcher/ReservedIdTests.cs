using System;
using System.Threading.Tasks;
using DomainBus.Dispatcher;
using DomainBus.Dispatcher.Client;
using FluentAssertions;
using NSubstitute;
using Tests.Processing.Saga;
using Xunit;

namespace Tests.Dispatcher
{
    public class ReservedIdTests
    {
        private MessageDispatcher _sut;
        private IStoreReservedMessagesIds _store;

        public ReservedIdTests()
        {
            _store = Substitute.For<IStoreReservedMessagesIds>();
            _sut =new MessageDispatcher(i=>Task.WhenAll(),_store);
        }

        [Fact]
        public void reserver_ids()
        {
            var id = Guid.NewGuid();
            Guid[] ids = null;
            _store.Add(Arg.Any<ReservedIdsSource>(),Arg.Do<Guid[]>(d=>ids=d));
            var rez = _sut.ReserveIdsFor(typeof (Handler), id, 2);
            _store.Received(1).Add(Arg.Any<ReservedIdsSource>(),rez);
            _store.Get(Arg.Any<ReservedIdsSource>()).Returns(ids);
            var rez2 = _sut.ReserveIdsFor(typeof(Handler), id, 2);
            _store.ReceivedWithAnyArgs(1).Add(Arg.Any<ReservedIdsSource>(), rez);
            rez.ShouldAllBeEquivalentTo(rez2);
        }

    }
}