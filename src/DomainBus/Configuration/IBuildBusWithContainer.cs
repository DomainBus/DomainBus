using System;

namespace DomainBus.Configuration
{
    public interface IBuildBusWithContainer
    {
      
        IBuildBusWithContainer ServerComunication(Action<IConfigureDispatcher> cfg);

        IBuildBusWithContainer CurrentHost(Action<IConfigureHost> cfg);

        IDomainBus Build(bool start = true);
    }

}