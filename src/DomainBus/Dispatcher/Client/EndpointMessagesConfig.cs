using DomainBus.Configuration;

namespace DomainBus.Dispatcher.Client
{
    public class EndpointMessagesConfig
    {
        public EndpointId Endpoint { get; set; }
        public string[] MessageTypes { get; set; }

        public EndpointMessagesConfig()
        {
            MessageTypes = new string[0];
        }
    }
}