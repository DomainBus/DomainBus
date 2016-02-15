namespace DomainBus
{
    public interface IAmStartedBy<T> : ISubscribeTo<T> where T : IEvent
    {
    }
}