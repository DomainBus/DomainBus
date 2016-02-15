using System;

namespace DomainBus.Configuration
{
    public interface IConfigureHostedEndpoint
    {
        bool CanHandle(Type type);
        /// <summary>
        /// Processor name
        /// </summary>
        string Name { get; }
    }
}