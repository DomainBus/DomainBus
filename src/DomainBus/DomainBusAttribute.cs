using System;

namespace DomainBus
{
    public class DomainBusAttribute:Attribute
    {
        public DomainBusAttribute(string endpointName)
        {
            EndpointName = endpointName;
        }

        public string EndpointName { get; set; }

    }
}