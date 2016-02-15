using DomainBus.Dispatcher.Client;
using DomainBus.Transport;

namespace DomainBus.Configuration
{
    public interface IConfigureDispatcher
    {
       IConfigureDispatcher TalkUsing(ITalkToServer communicator);
       IConfigureDispatcher ReceiveMessagesUsing(IReceiveServerMessages receiver);
    }
}