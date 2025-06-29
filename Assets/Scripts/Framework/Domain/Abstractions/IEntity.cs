namespace Elder.Framework.Domain.Abstractions
{
    public interface IEntity<TId>
    {
        public TId Id { get; }
    }
}
