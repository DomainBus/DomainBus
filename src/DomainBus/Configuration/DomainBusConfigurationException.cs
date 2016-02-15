using System;

namespace DomainBus.Configuration
{
    public class DomainBusConfigurationException : Exception
    {
        public DomainBusConfigurationException(EndpointId id):base("Endpoint {0} is already registered".ToFormat(id))
        {
            
        }

        public DomainBusConfigurationException(string msg):base(msg)
        {
            
        }
    }
}