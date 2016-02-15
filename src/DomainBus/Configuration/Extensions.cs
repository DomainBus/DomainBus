using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainBus.Configuration
{
    internal static class Extensions
    {
         public static bool CanHandleType(this IEnumerable<Type> types, Type handlerType)
         {
             if (types.IsNullOrEmpty()) return true;
             
             return types.Any(h => h == handlerType);
             
         }
       
    }
}