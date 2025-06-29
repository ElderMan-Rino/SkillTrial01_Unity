using Elder.Framework.Common.Utils;
using System;

namespace Elder.Framework.Asset.App
{
    internal readonly struct AssetKey : IEquatable<AssetKey>
    {
        private readonly int _typeHash;
        private readonly int _nameHash;

        public AssetKey(Type type, string name)
        {
            _typeHash = type.GetHashCode();
            _nameHash = StringHashHelper.ToStableHash(name);
        }

        public bool Equals(AssetKey other) => _typeHash == other._typeHash && _nameHash == other._nameHash;

        public override bool Equals(object obj) => obj is AssetKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(_typeHash, _nameHash);
    }
}