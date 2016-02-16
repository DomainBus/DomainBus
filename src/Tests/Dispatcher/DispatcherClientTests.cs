using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainBus;
using DomainBus.Abstractions;
using DomainBus.Audit;
using DomainBus.Configuration;
using DomainBus.Dispatcher;
using DomainBus.Dispatcher.Client;
using DomainBus.Transport;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Tests.Dispatcher
{
    public class DispatcherClientTests
    {
        private DispatcherClient _sut;
        private ITalkToServer _server;
        private IDeliveryErrorsQueue _err;
        private IEndpointConfiguration _config;


        public DispatcherClientTests()
        {
            _server = Substitute.For<ITalkToServer>();
            _err = Substitute.For<IDeliveryErrorsQueue>();
            _sut =new DispatcherClient(Setup.TestEndpoint.Host,_server,_err,new BusAuditor(new InMemoryAuditStorage()));
            _config = SetupFakeConfig();
            _sut.SubscribeToServer(new []{_config});
        }

        IEndpointConfiguration SetupFakeConfig()
        {
            var config = Substitute.For<IEndpointConfiguration>();
            config.Id.Returns(Setup.TestEndpoint);
            config.HandledMessagesTypes.Returns(new[] {typeof (MyEvent), typeof (MyCommand)});
            return config;
        }

        //class EpConfig : IEndpointConfiguration
        //{
        //    public EndpointId Id => Setup.TestEndpoint;
        //    public Type[] HandledMessagesTypes { get; set; } = {typeof (MyEvent), typeof (MyCommand)};
        //    public Task AddToProcessing(params IMessage[] messages)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        [LocalOnly]
        public class LocalEvent : AbstractEvent
        {
            
        }

        [Fact]
        public async Task messages_are_sent_local_and_server()
        {
            var myEvent = new MyEvent();
            var command = new MyCommand();
            EnvelopeFromClient env = null;
            await _server.SendMessages(Arg.Do<EnvelopeFromClient>(e => env = e));

            await _sut.Dispatch(myEvent,command);
            await _config.Received(1).AddToProcessing(Arg.Any<MyEvent>(), Arg.Any<MyCommand>());
            await _server.ReceivedWithAnyArgs(1).SendMessages(null);

            env.From.Should().Be(Setup.TestEndpoint.Host);
            env.Messages.ShouldAllBeEquivalentTo(new IMessage[] {myEvent,command});
        }

        [Fact]
        public async Task local_mesages_are_not_sent_to_server()
        {
            await _sut.Dispatch(new LocalEvent());
            await _server.DidNotReceiveWithAnyArgs().SendMessages(null);
        }

        [Fact]
        public void client_send_endpoint_info_to_server()
        {
            _server.Received(1).SendEndpointsConfiguration(Arg.Any<IEnumerable<EndpointMessagesConfig>>());
        }

        [Fact]
        public async Task if_server_communicator_throws_error_is_sent_to_err_queue()
        {
            var exception = new CouldntSendMessagesException(new EnvelopeFromClient(), "", new Exception());
            _server.SendMessages(Arg.Any<EnvelopeFromClient>()).Throws(exception);
            await _sut.Dispatch(new MyEvent());
            _err.Received(1).TransporterError(exception);
        }

        [Fact]
        public async Task receive_message_from_server()
        {
         
            var to = new EnvelopeToClient() {To=Setup.TestEndpoint,Messages = new [] {new MyEvent() }};
            await _sut.DeliverToLocalProcessors(to).ConfigureFalse();
            await _config.Received(1).AddToProcessing(Arg.Any<MyEvent>());
        }

        [Fact]
        public async Task receiving_for_unkown_endpoint_sents_to_err_queue()
        {
            var to = new EnvelopeToClient() { To = new EndpointId("other","locat"), Messages = new[] { new MyEvent() } };
            await _sut.DeliverToLocalProcessors(to).ConfigureFalse();
            _err.Received(1).UnknownEndpoint(Arg.Any<EndpointNotFoundException>());
            await _config.DidNotReceive().AddToProcessing(Arg.Any<MyEvent>());
        }

        [Fact]
        public async Task received_messages_are_not_sent_to_server()
        {
            var to = new EnvelopeToClient() { To = new EndpointId("other", "locat"), Messages = new[] { new MyEvent() } };
            await _sut.DeliverToLocalProcessors(to).ConfigureFalse();
            await _server.DidNotReceiveWithAnyArgs().SendMessages(null);
        }

        [Fact]
        public async Task received_but_unkown_messages_are_rejected()
        {
            var to = new EnvelopeToClient() { To = Setup.TestEndpoint, Messages = new IMessage[] { new MyEvent(),new LocalEvent()  } };
            await _sut.DeliverToLocalProcessors(to);
            await _config.Received(1).AddToProcessing(Arg.Any<MyEvent>());
            await _config.DidNotReceive().AddToProcessing(Arg.Any<LocalEvent>());
            _err.Received(1).MessagesRejected(Setup.TestEndpoint,Arg.Any<LocalEvent>());
        }
    }
}