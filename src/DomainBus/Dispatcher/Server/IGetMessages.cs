namespace DomainBus.Dispatcher.Server
{
    public interface IGetMessages
    {
        void Subscribe(IRouteMessages router);
    }
}