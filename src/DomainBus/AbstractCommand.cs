using DomainBus.Abstractions;

namespace DomainBus
{
    public abstract class AbstractCommand :BaseMessage, ICommand
    {
        protected AbstractCommand()
        {
            OperationId = Id;
            
        }

    }
}