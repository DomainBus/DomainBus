namespace DomainBus.Dispatcher.Server
{
    /// <summary>
    /// Singleton
    /// </summary>
    public interface IGetEndpointUpdates
    {
        void Subscribe(IWantEndpointUpdates server);
        void Start();
    }
}