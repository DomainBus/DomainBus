using System;
using System.Threading.Tasks;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Client;
using DomainBus.Dispatcher.Server;
using DomainBus.Transport;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Tests.Transport
{
    public class ServerMessagesReceiverTests:IDisposable
    {
        private FakeServerMessageReceiver _sut;
        private IDispatchReceivedMessages _router;

        public ServerMessagesReceiverTests()
        {
            LogManager.OutputToTrace();
            _sut=new FakeServerMessageReceiver();
            _sut.Add();
            _sut.PollingInterval = 50.ToMiliseconds();
            _router = Substitute.For<IDispatchReceivedMessages>();
        }

        [Fact]
        public async Task works_as_intended()
        {
            _sut.StartReceiving(_router);          
            await Task.Delay(150);
            _router.Received(1).DeliverToLocalProcessors(_sut.Envelopes[0]);
            _sut.Handled.Should().Be(_sut.Envelopes[0]);

        }

        [Fact]
        public async Task exceptions_thrown_by_router_dont_break_other_messages()
        {
            _router.DeliverToLocalProcessors(_sut.Envelopes[0]).Throws(new Exception("router exception"));
            _sut.Add();
            _sut.StartReceiving(_router);
            _sut.Start();
            await Task.Delay(150);

            _router.Received(2).DeliverToLocalProcessors(Arg.Any<EnvelopeToClient>());
            _sut.Handled.Should().Be(_sut.Envelopes[1]);
        }

        public void Dispose()
        {
            _sut.Dispose();
        }
    }
}