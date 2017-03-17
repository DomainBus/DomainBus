using System;
using System.Threading.Tasks;
using CavemanTools.Infrastructure;

namespace DomainBus
{
    internal class BusWithCommandResultMediator : IRequestCommandResult
    {
        private readonly IDomainBus _bus;
        private readonly ICommandResultMediator _med;

        public BusWithCommandResultMediator(IDomainBus bus,ICommandResultMediator med)
        {
            _bus = bus;
            _med = med;
        }

        public async Task<CommandResult> Send<T>(T cmd,TimeSpan? timeout=null) where T : ICommand
        {
            var res = _med.GetListener(cmd.Id,timeout);
            await _bus.GetDispatcher().SendAsync(cmd).ConfigureFalse();
            return await res.GetResult<CommandResult>();
        }
    }
}