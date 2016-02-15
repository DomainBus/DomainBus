
namespace DomainBus
{
    public interface ISubscribeTo<T> where T : IEvent
    {
        void Handle(T evnt);
    }
}