using DomainBus.Abstractions;

namespace DomainBus.Processing.Internals
{
    public interface IProcessMessage
    {
        /// <summary>
        /// Should never throw exceptions
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="processor"></param>
        void Process(IMessage msg, string processor);
    }
}