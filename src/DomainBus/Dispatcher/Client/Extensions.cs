using System.Collections.Generic;
using System.Linq;
using DomainBus.Abstractions;

namespace DomainBus.Dispatcher.Client
{
    public static class Extensions
    {
        internal static IMessage[] Filter(this IEndpointConfiguration ep, IEnumerable<IMessage> all)
            => all.Where(m => ep.HandledMessagesTypes.Contains(m.GetType())).ToArray();


    }
}