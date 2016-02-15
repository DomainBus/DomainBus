namespace DomainBus.Dispatcher.Server
{
    /// <summary>
    /// used as a singleton
    /// </summary>
    public interface IStoreDispatcherServerState
    {
        DispatcherState Load();
        void Save(DispatcherState state);
    }
}