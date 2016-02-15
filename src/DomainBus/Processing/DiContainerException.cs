using System;

namespace DomainBus.Processing
{
    public class DiContainerException:Exception
    {
        public Type HandlerType { get;}

        public DiContainerException(Type handlerType,Exception inner):base("",inner)
        {
            HandlerType = handlerType;
        }
    }
}