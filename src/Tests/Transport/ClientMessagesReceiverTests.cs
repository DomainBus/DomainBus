﻿using System;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Server;
using DomainBus.Transport;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Transport
{
    public class ClientMessagesReceiverTests:IDisposable
    {
        private readonly ITestOutputHelper _log;
        private FakeClientMEssagesReceiver _sut;
        private IRouteMessages _router;

        public ClientMessagesReceiverTests(ITestOutputHelper log)
        {
            _log = log;
            _sut=new FakeClientMEssagesReceiver();
            _sut.Add(); //add one envelope
            _router = Substitute.For<IRouteMessages>();
        }

        [Fact]
        public async Task works_as_intended()
        {
            _sut.Subscribe(_router);
            _sut.Start();
            _sut.Next();            
            await _router.Received(1).Route(_sut.Envelopes[0]);
            _sut.Handled.Should().Be(_sut.Envelopes[0]);

        }

        [Fact]
        public async Task exceptions_thrown_by_router_dont_break_other_messages()
        {

            _router.Route(_sut.Envelopes[0]).Throws(new Exception("router exception"));
            _sut.Add();
            _sut.Subscribe(_router);
            _sut.Start();
            _sut.Next();

              await _router.Received(2).Route(Arg.Any<EnvelopeFromClient>());
             _sut.Handled.Should().Be(_sut.Envelopes[1]);
        }

        public void Dispose()
        {
            _sut.Dispose();
        }
    }
}