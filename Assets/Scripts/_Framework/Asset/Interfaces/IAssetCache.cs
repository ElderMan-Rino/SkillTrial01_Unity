using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetCache : IFrameworkComponent
    {
        public UniTask<IAssetHandle> GetAssetAsync<T>(string key) where T : UnityEngine.Object;
        public void UnregisterKey(string key);
        public UniTask RegisterLabelAsync(string label);
        public void UnregisterLabel(string label);
    }
}
