using System;
using System.Collections.Generic;
using System.Linq;

namespace Elder.Framework.Domain.Abstractions
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public bool Equals(ValueObject other)
        {
            if (other is null) return false;
            if (GetType() != other.GetType()) return false;
            // [HEAP] SequenceEqual — LINQ 열거자 할당, 비교 빈도가 낮은 도메인 로직에서만 사용
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override bool Equals(object obj) => obj is ValueObject other && Equals(other);

        public override int GetHashCode()
        {
            // [HEAP] Aggregate — LINQ 열거자 할당, GetHashCode 호출 빈도가 낮은 경우에만 사용
            return GetEqualityComponents()
                .Aggregate(1, (hash, obj) => HashCode.Combine(hash, obj?.GetHashCode() ?? 0));
        }

        public static bool operator ==(ValueObject left, ValueObject right) =>
            left?.Equals(right) ?? right is null;

        public static bool operator !=(ValueObject left, ValueObject right) => !(left == right);
    }
}
