using CavemanTools.Infrastructure;
using CavemanTools.Logging;
using DomainBus;
using DomainBus.Processing;
using DomainBus.Processing.Internals;
using NSubstitute;

namespace Tests.Processing.Saga
{
    public abstract class BaseSagaTests
    {
       
        protected IHandlerTypeInvoker _invoker;
        protected IContainerScope _di;
        protected IDispatchMessages _bus;
        protected Handler _handler;
        protected IStoreSagaState _storage;
        
        protected MySagaState _saga;

        public BaseSagaTests()
        {
            LogManager.OutputToTrace();
            _invoker = Substitute.For<IHandlerTypeInvoker>();
            _di = Substitute.For<IContainerScope>();
            _bus = Substitute.For<IDispatchMessages>();
            _handler = new Handler(_bus);
            _storage = Substitute.For<IStoreSagaState>();
            _saga = new MySagaState();

            this.Setup();

           
        }

        private void Setup()
        {
            _di.BeginLifetimeScope().Returns(_di);
            _di.Resolve<IStoreSagaState>().Returns(_storage);
            _di.Resolve(typeof(MySagaState)).Returns(_saga);

            _storage.GetSaga(Arg.Any<string>(), typeof (MySagaState)).Returns(_saga);
            _invoker.HandlerType.Returns(typeof(Handler));
            _invoker.InstantiateHandler(_di).Returns(_handler);

        }
    }
}