using System.Collections.Generic;
using DomainBus.Dispatcher.Client;
using DomainBus.Transport;
using Ploeh.AutoFixture;

namespace Tests.Transport
{
    public class FakeEndpointConfigurationReceiver:AEndpointConfigurationReceiver
    {
        public List<EndpointMessagesConfig> Configs { get; }=new List<EndpointMessagesConfig>();
        public FakeEndpointConfigurationReceiver()
        {
            Configs.AddRange(Setup.Fixture.CreateMany<EndpointMessagesConfig>());
        }


        protected override EndpointMessagesConfig[] GetConfigs()
        {
           Stop();
           return Configs.ToArray();
        }

        public IEnumerable<EndpointMessagesConfig> Handled { get; private set; }

        protected override void MarkAsHandled(IEnumerable<EndpointMessagesConfig> configs)
        {
            Handled = configs;
        }
    }
}