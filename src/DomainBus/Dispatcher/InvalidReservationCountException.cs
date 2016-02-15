using System;

namespace DomainBus.Dispatcher
{
    public class InvalidReservationCountException : Exception
    {
        public Guid MessageId { get; set; }
        public Type Type { get; set; }
        public int Expected { get; set; }
        public int Actual { get; set; }

        public InvalidReservationCountException(Guid messageId,Type type, int expected, int actual):base("There is a prior reservation of {2} ids for handler {0} with message {1},but now it requested {3}".ToFormat(type,messageId,expected,actual))
        {
            MessageId = messageId;
            Type = type;
            Expected = expected;
            Actual = actual;
        }
    }
}