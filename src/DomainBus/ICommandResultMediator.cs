using System;
using CavemanTools.Infrastructure;

namespace DomainBus
{
    /// <summary>
    /// This should be a dependency for a command handler which returns a command result
    /// </summary>
    public interface IReturnCommandResult
    {
        /// <summary>
        /// This is how the command handler returns a result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdId"></param>
        /// <param name="result"></param>
        void AddResult<T>(Guid cmdId, T result) where T :class;
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ICommandResultMediator : IReturnCommandResult
    {
        /// <summary>
        /// Used to await a command result
        /// </summary>
        /// <param name="cmdId"></param>
        /// <param name="timeout">Default is 5s</param>
        /// <returns></returns>
        IResultListener GetListener(Guid cmdId,TimeSpan? timeout=null);
    }
}