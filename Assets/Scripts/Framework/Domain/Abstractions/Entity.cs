using System;
using System.Collections.Generic;

namespace Elder.Framework.Domain.Abstractions
{
    public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
    {
        public TId Id { get; protected set; }

        protected Entity(TId id)
        {
            Id = id;
        }

        public bool Equals(Entity<TId> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object obj) => obj is Entity<TId> other && Equals(other);

        public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id);

        public static bool operator ==(Entity<TId> left, Entity<TId> right) =>
            left?.Equals(right) ?? right is null;

        public static bool operator !=(Entity<TId> left, Entity<TId> right) => !(left == right);
    }
}
