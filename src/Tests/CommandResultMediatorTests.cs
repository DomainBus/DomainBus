﻿using System;
using System.Threading.Tasks;
using CavemanTools.Infrastructure;
using DomainBus;
using FluentAssertions;
using Xunit;

namespace Tests.Infrastructure
{

    public class CommandResult
    {
         
    }
    public class CommandResultMediatorTests
    {
        public CommandResultMediatorTests()
        {

        }

        [Fact]
        public async Task ok_scenario()
        {
            var m = new CommandResultMediator();
            var l = m.GetListener(Guid.Empty);

            m.ActiveListeners.Should().Be(1);


            await Task.Run(() =>
            {
                this.Sleep(TimeSpan.FromMilliseconds(840));
                m.AddResult(Guid.Empty, new CommandResult());
            });

            var r = await l.GetResult<CommandResult>().ConfigureAwait(false);
            r.Should().NotBeNull();
            m.ActiveListeners.Should().Be(0);
        }

        [Fact]
        public  async Task timeout_throws()
        {
            var m = new CommandResultMediator();
            var l = m.GetListener(Guid.Empty, TimeSpan.FromMilliseconds(250));
          

            await Assert.ThrowsAsync<TimeoutException>(() => l.GetResult<CommandResult>());
      
        }


        [Fact]
        public async Task if_timeout_then_listener_is_removed()
        {
            var m = new CommandResultMediator();
            var l = m.GetListener(Guid.Empty, TimeSpan.FromMilliseconds(250));
            m.ActiveListeners.Should().Be(1);

           try
            {
                await l.GetResult<CommandResult>();
                throw new Exception("Should not be thrown");
            }
            catch (TimeoutException)
            {
                m.ActiveListeners.Should().Be(0);
            }
            
        }
    }
}