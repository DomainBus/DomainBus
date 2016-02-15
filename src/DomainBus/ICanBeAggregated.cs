namespace DomainBus
{
    public interface ICanBeAggregated<TSelf> where TSelf:IEvent
    {
        TSelf Aggregate(TSelf other);
    }
}