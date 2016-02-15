namespace DomainBus.Configuration
{
    public interface IManageProcessingService : IConfigureProcessingService
    {
        bool IsPaused { get; }

        string Name { get; }

        void Start();
        void Stop();

        /// <summary>
        /// Blocks thread until all workers complete their work
        /// </summary>
        void WaitUntilWorkersFinish();
    }
}