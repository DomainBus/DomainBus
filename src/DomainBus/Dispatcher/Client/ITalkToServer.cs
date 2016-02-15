using System.Collections.Generic;
using System.Threading.Tasks;
using DomainBus.Transport;

namespace DomainBus.Dispatcher.Client
{
    public interface ITalkToServer
    {
        void SendEndpointsConfiguration(IEnumerable<EndpointMessagesConfig> data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="envelope"></param>
        /// <exception cref="DomainBus.Transport.CouldntSendMessagesException"></exception>
        /// <returns></returns>
        Task SendMessages(EnvelopeFrom envelope);

    }
}