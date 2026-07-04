using Unity.Collections;
using UnityEngine;

namespace Elder.Framework.Asset.Definitions
{
    public readonly struct PlatformAddressableRule
    {
        public readonly int Id;
        public readonly FixedString64Bytes Key;
        public readonly RuntimePlatform TargetPlatform;
        public readonly FixedString64Bytes AddressableLabel;

        public PlatformAddressableRule(int id, FixedString64Bytes key, RuntimePlatform targetPlatform, FixedString64Bytes addressableLabel)
        {
            Id = id;
            Key = key;
            TargetPlatform = targetPlatform;
            AddressableLabel = addressableLabel;
        }
    }
}
