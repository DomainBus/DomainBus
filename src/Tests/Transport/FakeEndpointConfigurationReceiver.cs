using System.Collections.Generic;
using CavemanTools.Testing;
using DomainBus.Dispatcher.Client;
using DomainBus.Transport;


namespace Tests.Transport
{
    public class FakeEndpointConfigurationReceiver:AEndpointConfigurationReceiver
    {
        public List<EndpointMessagesConfig> Configs { get; }=new List<EndpointMessagesConfig>();
        public FakeEndpointConfigurationReceiver():base(new StubTimer())
        {
            Configs.AddRange(new EndpointMessagesConfig[]{new EndpointMessagesConfig(){}, });
        }



        protected override EndpointMessagesConfig[] GetConfigs()
        {
           return Configs.ToArray();
        }

        public IEnumerable<EndpointMessagesConfig> Handled { get; private set; }

        protected override void MarkAsHandled(IEnumerable<EndpointMessagesConfig> configs)
        {
            Handled = configs;
        }
    }
}