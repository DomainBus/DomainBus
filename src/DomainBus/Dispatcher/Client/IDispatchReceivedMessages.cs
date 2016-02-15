using System.Threading.Tasks;
using DomainBus.Transport;

namespace DomainBus.Dispatcher.Client
{
    public interface IDispatchReceivedMessages
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="EndpointNotFoundException"></exception>
        /// <param name="envelope"></param>
        /// <returns></returns>
        Task DeliverToLocalProcessors(EnvelopeTo envelope);
    }
}


