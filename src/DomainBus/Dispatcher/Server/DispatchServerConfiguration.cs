using System;
using DomainBus.Abstractions;
using DomainBus.Configuration;
using DomainBus.Transport;

namespace DomainBus.Dispatcher.Server
{
    public class DispatchServerConfiguration
    {
        public IStoreDispatcherServerState Storage { get; set; }
        public IGetEndpointUpdates EndpointUpdatesNotifier { get; set; }
        public IGetMessages MessageNotifier { get; set; }
        public IDeliveryErrorsQueue DeliveryErrorsQueue { get; set; }
        public TransportersHub Transporters { get; }=new TransportersHub();

        public void Validate()
        {
            Storage.MustNotBeNull();
            EndpointUpdatesNotifier.MustNotBeNull();
            MessageNotifier.MustNotBeNull();
            DeliveryErrorsQueue.MustNotBeNull();           
        }
    }

    public static class Extensions
    {
       /// <summary>
       /// No more intermediaries, messages go straight to the processor.
       /// </summary>
       /// <param name="hub"></param>
       /// <param name="id"></param>
       /// <param name="factory"></param>
        public static void SendDirectlyToProcessingStorage(this TransportersHub hub,EndpointId id ,Func<IAddMessageToProcessorStorage> factory) 
            => hub.Add(id,new ProcessingStorageTransporter(factory,id));
    }
}