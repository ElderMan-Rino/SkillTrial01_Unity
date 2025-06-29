using Cysharp.Threading.Tasks;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetProvider
    {
        public UniTask<IAssetHandle<T>> GetAssetAsync<T>(string key) where T : UnityEngine.Object;
    }
}