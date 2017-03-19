using System;
using System.Linq;
using System.Threading.Tasks;
using DomainBus;
using DomainBus.Abstractions;
using DomainBus.Configuration;
using DomainBus.Dispatcher.Client;
using DomainBus.Dispatcher.Server;
using DomainBus.Transport;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Tests.Dispatcher
{
    public class ServerTests
    {
        private DispatcherServer _sut;
        private IDeliveryErrorsQueue _err;
        private IDeliverToEndpoint _transporter;

        public ServerTests()
        {
            _err = Substitute.For<IDeliveryErrorsQueue>();
            _transporter = Substitute.For<IDeliverToEndpoint>();
            _sut =new DispatcherServer(c =>
            {
                c.DeliveryErrorsQueue = _err;
                c.Storage = Substitute.For<IStoreDispatcherServerState>();
                c.Transporters.Add(Setup.TestEndpoint,_transporter);
                c.EndpointUpdatesNotifier = Substitute.For<IGetEndpointUpdates>();
                c.MessageNotifier = Substitute.For<IGetMessages>();
            });

            _sut.ReceiveConfigurations(new [] {SetupConfig()});
            _sut.Start();
        }

        EndpointMessagesConfig SetupConfig()
        {
            return new EndpointMessagesConfig()
            {
                Endpoint = Setup.TestEndpoint,
                MessageTypes = new[] {typeof(MyEvent),typeof(MyCommand)}.Select(d=>d.AsMessageName()).ToArray()
            };
        }


        public class OtherEvent:AbstractEvent
        {
             
        }

        [Fact]
        public async Task routing_messages_for_each_endpoint()
        {
            var myEvent = new MyEvent();
            var env=new EnvelopeFromClient()
            {
                From ="some host",
                Messages = new IMessage[] {myEvent,new OtherEvent()}
            };

            EnvelopeToClient dest = null;
            await _transporter.Send(Arg.Do<EnvelopeToClient>(e => dest = e));

            await _sut.Route(env);

            dest.To.Should().Be(Setup.TestEndpoint);
            dest.Messages.ShouldAllBeEquivalentTo(new[] {myEvent});

        }

        [Fact]
        public async Task messages_arent_sent_back_to_origin()
        {
            var myEvent = new MyEvent();
            var env=new EnvelopeFromClient()
            {
                From =Setup.TestEndpoint.Host,
                Messages = new IMessage[] {myEvent,new OtherEvent()}
            };

            await _sut.Route(env);

            await _transporter.DidNotReceiveWithAnyArgs().Send(null);

        }

        [Fact]
        public async Task if_transport_doesnt_exist_nothing_happens()
        {
            var env = new EnvelopeFromClient()
            {
                From = Setup.TestEndpoint.Host,
                Messages = new IMessage[] { new MyEvent(), new OtherEvent() }
            };
            _sut.ReceiveConfigurations(new [] {new EndpointMessagesConfig()
            {
                Endpoint = new EndpointId("other","local")
                ,MessageTypes = new [] {typeof(OtherEvent).AsMessageName()}
            } });

            await _sut.Route(env);
        }

        [Fact]
        public async Task transporter_error_goes_to_err_queue()
        {
            var exception = new CouldntSendMessagesException(new EnvelopeToClient(), "", new Exception());
            _transporter.Send(Arg.Any<EnvelopeToClient>()).Throws(exception);
            var env = new EnvelopeFromClient()
            {
                From = "other",
                Messages = new IMessage[] { new MyEvent(), new OtherEvent() }
            };
            await _sut.Route(env);
            _err.Received(1).TransporterError(exception);
        }
    }
}