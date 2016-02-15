using System;

namespace DomainBus.Processing
{
    public class BusStorageException : Exception
    {
        public BusStorageException()
        {
            
        }
        public BusStorageException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}