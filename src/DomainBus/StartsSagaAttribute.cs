using System;

namespace DomainBus
{
    /// <summary>
    /// Marks a handler method as a saga starter
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StartsSagaAttribute:Attribute
    {
         
    }
}