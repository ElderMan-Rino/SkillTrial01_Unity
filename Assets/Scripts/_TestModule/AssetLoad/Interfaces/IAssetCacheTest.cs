using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Definitions;
using Elder.Framework.Asset.Interfaces;
using System.Collections.Generic;

namespace Elder.TestModule.AssetLoad.Interfaces
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
