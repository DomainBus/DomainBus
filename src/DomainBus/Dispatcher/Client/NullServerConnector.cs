using System.Collections.Generic;
using System.Threading.Tasks;
using DomainBus.Transport;

namespace DomainBus.Dispatcher.Client
{
    public class NullServerConnector:ITalkToServer
    {
        public static readonly NullServerConnector Instance=new NullServerConnector();
        public void SendEndpointsConfiguration(IEnumerable<EndpointMessagesConfig> data)
        {
            
        }

        public Task SendMessages(EnvelopeFrom envelope) => TasksUtils.EmptyTask();
    }
}