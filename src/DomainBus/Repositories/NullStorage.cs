using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainBus.Abstractions;
using DomainBus.Audit;
using DomainBus.Configuration;
using DomainBus.Dispatcher;
using DomainBus.Processing;
using DomainBus.Transport;

namespace DomainBus.Repositories
{
    public class NullStorage:IStoreAudits,IStoreReservedMessagesIds
       
        ,IStoreUnhandledMessages
        ,IStoreSagaState
        ,IFailedMessagesQueue
        ,IDeliveryErrorsQueue
    {
        public static NullStorage Instance=new NullStorage();
        private NullStorage()
        {
            
        }

        public void Append(params MessageTransportAudit[] audits)
        {
            
        }

        public void Append(params MessageRejectedAudit[] audits)
        {
            
        }

        public void Append(MessageProcessingAudit audits)
        {
           
        }

        public void Append(params StoredMessageAudit[] audits)
        {
           
        }

        public void Dispose()
        {
         
        }

        public Task Add(string queueId, IEnumerable<IMessage> items)
            => TasksUtils.EmptyTask();


        public Guid[] Get(ReservedIdsSource input)
            => Array.Empty<Guid>();

        public void Add(ReservedIdsSource id, Guid[] ids)
        {
          
        }

   
   

        public IEnumerable<IMessage> GetMessages(string queueId, int take)
        =>Enumerable.Empty<IMessage>();

        public void MarkMessageHandled(string queue, Guid id)
        {
           
        }

        public ISagaState GetSaga(string correlationId, Type sagaStateType)
        {
            throw new NotImplementedException();
        }

        public void Save(ISagaState data, string correlationId, bool isNew)
        {
           
        }

        public void MessageHandlingFailed(IMessage msg, HandledMessageException ex)
        {
           
        }

        public void MessageCantBeHandled(IMessage msg, DiContainerException ex)
        {
           
        }

        public void MessageCantBeHandled(IMessage msg, SagaConfigurationException ex)
        {
           
        }

        public void MessageCantBeHandled(IMessage msg, MissingHandlerException ex)
        {
            
        }

        public void TransporterError(CouldntSendMessagesException ex)
        {
          
        }

        public void UnknownEndpoint(EndpointNotFoundException ex)
        {
            
        }

        public void MessagesRejected(EndpointId receiver, params IMessage[] msg)
        {
           
        }
    }
}