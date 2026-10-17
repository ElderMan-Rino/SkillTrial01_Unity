using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Definitions;
using System.Collections.Generic;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetCacheTest : IAssetCache
    {
        public UniTask RegisterAllKeysAsync();
        public UniTask ReleaseAllAsync();
        public IReadOnlyCollection<string> GetCachedKeys();
        public IReadOnlyCollection<string> GetRegisteredLabels();
        public AssetCacheMemoryStatus GetMemoryStatus();
    }
}
