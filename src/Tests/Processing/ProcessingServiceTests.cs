using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CavemanTools.Logging;
using DomainBus.Abstractions;
using DomainBus.Audit;
using DomainBus.Processing;
using DomainBus.Processing.Internals;
using DomainBus.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Tests.Processing
{
    public class ProcessingServiceTests:IDisposable
    {
        private ProcessingService _sut;
        private IStoreUnhandledMessages _storage;
        private IProcessMessage _processor;
        private IFailedMessagesQueue _errors;

        public ProcessingServiceTests()
        {
            LogManager.OutputToTrace();
            _storage = Substitute.For<IStoreUnhandledMessages>();
            _processor = Substitute.For<IProcessMessage>();
            _errors = Substitute.For<IFailedMessagesQueue>();

            _sut =new ProcessingService(_storage,()=>_processor,new BusAuditor(new InMemoryAuditStorage()), _errors);
            _sut.PollingEnabled = false;
            _sut.Start();
        }

        [Fact]
        public async Task runs_normally()
        {
            var msg = new MyEvent();
            await _sut.Queue(msg);
            _sut.WaitUntilWorkersFinish();
            _processor.Received(1).Process(msg,_sut.Name);
            _storage.Received(1).MarkMessageHandled(_sut.Name,msg.Id);
        }

        [Fact]
        public async  Task when_data_storage_throws_service_doesnt_stop()
        {
            var msg = new MyEvent();
            _storage.When(d => d.MarkMessageHandled(Arg.Any<string>(), msg.Id)).Throw<BusStorageException>();
            await _sut.Queue(msg);
            _sut.WaitUntilWorkersFinish();

            var other = new MyEvent();
            var ok = false;
            _storage.When(d => d.MarkMessageHandled(Arg.Any<string>(), other.Id)).Do(i=>ok=true);
            await _sut.Queue(other);
            _sut.WaitUntilWorkersFinish();
            ok.Should().BeTrue();
        }

        [Fact]
        public async Task when_throwing_config_exceptions_error_queue_is_notified()
        {
            await this.Throw(new DiContainerException(null, null));
            await this.Throw(new MissingHandlerException(null));
            await this.Throw(new SagaConfigurationException(""));
        }

        private async Task Throw(Exception ex)
        {
            var msg = new MyEvent();
            _processor.When(d => d.Process(msg, Arg.Any<string>())).Throw((dynamic) ex);
            await _sut.Queue(msg);
            _sut.WaitUntilWorkersFinish();
            _errors.Received(1).MessageCantBeHandled(msg,(dynamic)ex);
        }

        [Fact]
        public void load_from_storage()
        {
            _sut.Stop();
            _sut.PollingEnabled = true;
            _sut.PollingInterval = 100.ToMiliseconds();
            
            var queue=new Queue<IMessage>();
            var myEvent = new MyEvent();
            queue.Enqueue(myEvent);
            var myCommand = new MyCommand();
            queue.Enqueue(myCommand);

            _storage.GetMessages(_sut.Name, Arg.Any<int>()).Returns(i => new [] { queue.Dequeue()});

            _sut.Start();
            _sut.WaitUntilWorkersFinish();
            _processor.Received(1).Process(myEvent,_sut.Name);
            _processor.Received(1).Process(myCommand,_sut.Name);
        }

        public void Dispose()
        {
            _sut.Dispose();
        }
    }
}