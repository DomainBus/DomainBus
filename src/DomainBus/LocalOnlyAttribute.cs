using System;

namespace DomainBus
{
    /// <summary>
    /// Specifies the message shouldn't be send to the server. Only to local processors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = true)]
    public class LocalOnlyAttribute : Attribute
    {
        
    }
}