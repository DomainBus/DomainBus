using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CavemanTools.Logging;
using DomainBus.Dispatcher.Client;
using DomainBus.Dispatcher.Server;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Tests.Transport
{
    public class EndpointConfigurationReceiverTests:IDisposable
    {
        private FakeEndpointConfigurationReceiver _sut;
        private IWantEndpointUpdates _server;

        public EndpointConfigurationReceiverTests()
        {
            LogManager.OutputToTrace();
            _sut=new FakeEndpointConfigurationReceiver();
            _server = Substitute.For<IWantEndpointUpdates>();
        }

        [Fact]
        public async Task it_works_properly()
        {
            _sut.Subscribe(_server);
            _sut.PollingInterval = 50.ToMiliseconds();
            _sut.Start();
            await Task.Delay(150);
            var configs = _sut.Configs.ToArray();
            _server.Received(1).ReceiveConfigurations(Arg.Any<IEnumerable<EndpointMessagesConfig>>());
            _sut.Handled.ShouldAllBeEquivalentTo(configs);
        }

        public void Dispose()
        {
            _sut.Dispose();
        }
    }
}