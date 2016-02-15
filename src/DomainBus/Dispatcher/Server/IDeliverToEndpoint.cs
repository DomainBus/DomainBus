using System.Threading.Tasks;
using DomainBus.Transport;

namespace DomainBus.Dispatcher.Server
{
    public interface IDeliverToEndpoint
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="CouldntSendMessagesException"></exception>
        /// <param name="envelope"></param>
        /// <returns></returns>
        Task Send(EnvelopeTo envelope);
    }
}