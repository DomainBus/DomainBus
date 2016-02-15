using DomainBus.Abstractions;

namespace DomainBus.Processing
{
    public interface IFailedMessagesQueue 
    {
        void MessageHandlingFailed(IMessage msg,HandledMessageException ex);
        void MessageCantBeHandled(IMessage msg,DiContainerException ex);
        void MessageCantBeHandled(IMessage msg,SagaConfigurationException ex);
        void MessageCantBeHandled(IMessage msg,MissingHandlerException ex);
       
    }
}