using System;
using System.Linq;
using DomainBus.Abstractions;
using DomainBus.Processing.Internals;
using FluentAssertions;
using Xunit;

namespace Tests.Processing
{
    public class ProcessingCacheTests
    {
        private ProcessorMessageCache _sut;
        private MyEvent _myEvent;

        public ProcessingCacheTests()
        {
            _sut= new ProcessorMessageCache();
            _myEvent = new MyEvent() {TimeStamp = DateTimeOffset.Now.AddHours(1)};
            _sut.Add(new IMessage[] {_myEvent});
        }

        [Fact]
        public void you_cant_add_same_messageid_twice()
        {
            _sut.Cache.Count().Should().Be(1);
            _sut.Add(new []{new MyEvent() {Id = _myEvent.Id} });
            _sut.Cache.Count().Should().Be(1);
        }

       
        [Fact]
        public void message_is_removed_from_cache()
        {
            var myCommand = new MyCommand();
            _sut.Add(new IMessage[1] {myCommand});
            _sut.Cache.Count().Should().Be(2);
            var first=_sut.GetNextMessage();
            first.Should().Be(_myEvent);
            _sut.MessageHandled(first);
            
            _sut.Cache.Count().Should().Be(1);
        }
    }
}