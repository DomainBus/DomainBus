using DomainBus.Dispatcher.Client;

namespace DomainBus.Transport
{

    /// <summary>
    /// Will be used as a singleton
    /// </summary>
    public interface IReceiveServerMessages
    {
        void StartReceiving(IDispatchReceivedMessages dispatcher);
        
    }
}