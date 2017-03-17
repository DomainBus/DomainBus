using System;
using System.Threading.Tasks;
using CavemanTools.Infrastructure;

namespace DomainBus
{
    /// <summary>
    /// Used to send a command and receive a command result.
    /// Replaces <see cref="IDispatchMessages"/> for the cases where you want an immediate result.
    /// Should be treated as singleton
    /// </summary>
    public interface IRequestCommandResult
    {
        /// <summary>
        /// Sends command then waits for its execution.
        /// To be used ONLY in handlers who execute in the same process as the caller.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="timeout">Default is 5 seconds</param>
        /// <returns></returns>
        Task<CommandResult> Send<T>(T cmd, TimeSpan? timeout = null) where T : ICommand;
    }
}