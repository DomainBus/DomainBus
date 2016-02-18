using System.Threading.Tasks;
using DomainBus.Transport;

namespace DomainBus.Dispatcher.Server
{
    public interface IRouteMessages
    {
        Task Route(EnvelopeFromClient envelope);      
    }
}