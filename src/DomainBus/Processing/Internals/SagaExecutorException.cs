using System;

namespace DomainBus.Processing.Internals
{
    public class SagaExecutorException : Exception
    {
        public SagaExecutorException(string msg):base(msg)
        {        
        }
    }
}