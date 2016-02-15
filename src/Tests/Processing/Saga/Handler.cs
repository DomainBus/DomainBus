using System;
using DomainBus;

namespace Tests.Processing.Saga
{
    public class Handler:Saga<MySagaState>
    {
        public Handler(IDispatchMessages bus)
        {
            this.MapCorrelationIdFrom<MyEvent>(e=>e.EntityId);
            this.MapCorrelationIdFrom<OtherEvent>(e=>e.EntityId);
            this.MapCorrelationIdFrom<StartEvent>(e=>e.EntityId);

            this.SagaCompletesWhen(t => t.Has("ev", "other"))
                .OnCompleteDo(d => bus.Send(new MyCommand()
                {
                    SomeId = d.Get<Guid>("started"),
                    OperationId = d.Id
                }));

        }

        [StartsSaga]
        public void Handle(StartEvent ev)
        {
            this.Set("started",ev.EntityId);

        }

        public void Handle(MyEvent ev)
        {
            this.Set("ev",true);
        }
        public void Handle(OtherEvent ev)
        {
            this.Set("other",true);
        }

    }
}