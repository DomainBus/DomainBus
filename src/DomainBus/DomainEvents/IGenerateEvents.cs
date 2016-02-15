
namespace DomainBus.DomainEvents
{
    public interface IGenerateEvents
    {
        IEvent[] GetGeneratedEvents();
        /// <summary>
        /// Removes all generated events
        /// </summary>
        void ClearEvents();
    }
}