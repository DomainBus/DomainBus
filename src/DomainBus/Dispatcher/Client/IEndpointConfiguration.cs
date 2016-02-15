using System;
using System.Threading.Tasks;
using DomainBus.Abstractions;
using DomainBus.Configuration;

namespace DomainBus.Dispatcher.Client
{
    public interface IEndpointConfiguration
    {
        EndpointId Id { get; }
        Type[] HandledMessagesTypes { get; }
        Task AddToProcessing(params IMessage[] messages);
    }
}