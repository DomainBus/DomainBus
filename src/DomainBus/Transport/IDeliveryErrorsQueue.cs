using DomainBus.Abstractions;
using DomainBus.Configuration;
using DomainBus.Dispatcher;

namespace DomainBus.Transport
{
    public interface IDeliveryErrorsQueue
    {
        /// <summary>
        /// Transporter couldn't deliver it
        /// </summary>
        void TransporterError(CouldntSendMessagesException ex);

        void UnknownEndpoint(EndpointNotFoundException ex);
        void MessagesRejected(EndpointId receiver, params IMessage[] msg);
    }
}