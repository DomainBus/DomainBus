namespace DomainBus.DomainEvents
{
    public abstract class AnaemicEntityCommand<T>:AbstractCommand where T :class
    {
        public T Entity { get; set; }
        
        protected AnaemicEntityCommand(T entity)
        {
            Entity = entity;            
        }
    }
}