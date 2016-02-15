using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CavemanTools;
using DomainBus.Abstractions;

namespace DomainBus.Processing.Internals
{
    public class EventHandlerSubscription : IInvokeHandler
    {
        private readonly List<IInvokeHandler> _handlers = new List<IInvokeHandler>();

        public List<IInvokeHandler> Handlers => _handlers;

        public void Add(IInvokeHandler eventHandler)
        {
            eventHandler.MustNotBeNull();
            Handlers.Add(eventHandler);          
        }

        private bool _sorted = false;

        public void Handle(IMessage msg,string processor)
        {
           SortHandlers();

            Handlers.ForEach(h=>h.Handle(msg,processor));

            //List<Exception> list = new List<Exception>();
            
            //foreach (var handler in Handlers)
            //{
            //    try
            //    {
            //         handler.Handle(msg,processor);
            //    }
              
            //    catch (HandledMessageException ex)
            //    {
                   
            //        list.Add(ex);
            //    }
            //}
            //if (list.Count > 0) throw new EventHandlingException(list);
        }

        private void SortHandlers()
        {
            if (_sorted) return;
            
            var sorted =
                _handlers.OrderBy(h => h.GetHandlersTypes().First().GetTypeInfo().GetAttributeValue<OrderAttribute, int>(o => o.Value))
                    .ToArray();
            _handlers.Clear();
            _handlers.AddRange(sorted);
            _sorted = true;
        }

        public IEnumerable<Type> GetHandlersTypes()
        {
            SortHandlers();
            return _handlers.SelectMany(h => h.GetHandlersTypes());
        }
    }
}