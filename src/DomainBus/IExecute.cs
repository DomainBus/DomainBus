namespace DomainBus
{
    public interface IExecute<T> where T : ICommand
    {
        void Execute(T cmd);
    }
}