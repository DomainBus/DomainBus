using System;

namespace DomainBus.Processing
{
    public class SagaConfigurationException : Exception
    {
        public SagaConfigurationException(string message) : base(message)
        {
        }
    }
}