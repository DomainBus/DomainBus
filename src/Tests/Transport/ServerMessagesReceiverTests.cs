using System;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Client;
using DomainBus.Dispatcher.Server;
using DomainBus.Transport;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Transport
{
    public class ServerMessagesReceiverTests:IDisposable
    {
        private readonly ITestOutputHelper _test;
        private FakeServerMessageReceiver _sut;
        private IDispatchReceivedMessages _router;

        public ServerMessagesReceiverTests(ITestOutputHelper test)
        {
            _test = test;

            _sut=new FakeServerMessageReceiver();
            _sut.Add();
            _router = Substitute.For<IDispatchReceivedMessages>();
        }

        [Fact]
        public void works_as_intended()
        {
            LogManager.OutputTo(_test.WriteLine);
            _sut.StartReceiving(_router);
            _sut.Next();
            
            _sut.Handled.Should().Be(_sut.Envelopes[0]);
            _router.Received(1).DeliverToLocalProcessors(_sut.Envelopes[0]);
            LogManager.OutputTo(Empty.ActionOf<string>());

        }

        [Fact]
        public async Task exceptions_thrown_by_router_dont_break_other_messages()
        {
            _router.DeliverToLocalProcessors(_sut.Envelopes[0]).Throws(new Exception("router exception"));
            _sut.Add();
            _sut.StartReceiving(_router);

            _sut.Next();

            _router.Received(2).DeliverToLocalProcessors(Arg.Any<EnvelopeToClient>());
              _sut.Handled.Should().Be(_sut.Envelopes[1]);
        }

        public void Dispose()
        {
            _sut.Dispose();
            LogManager.OutputTo(Empty.ActionOf<string>());
        }
    }
}