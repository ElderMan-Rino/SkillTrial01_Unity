using Elder.Framework.Domain.Abstractions;

namespace Elder.Framework.Domain.Repositories
{
    public interface IRepository<TAggregate, TId>
        where TAggregate : AggregateRoot<TId>
    {
        public TAggregate FindById(TId id);
        public void Add(TAggregate aggregate);
        public void Remove(TAggregate aggregate);
    }
}
