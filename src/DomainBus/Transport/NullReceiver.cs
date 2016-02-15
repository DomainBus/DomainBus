using DomainBus.Dispatcher.Client;

namespace DomainBus.Transport
{
    public class NullReceiver : IReceiveServerMessages
    {
        public static NullReceiver Instance=new NullReceiver();
        private NullReceiver()
        {
            
        }
        public void StartReceiving(IDispatchReceivedMessages dispatcher)
        {
            
        }
    }
}