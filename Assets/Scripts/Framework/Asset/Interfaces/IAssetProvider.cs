using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetProvider : IGameSystem
    {
        public UniTask<IAssetHandle<T>> GetAssetAsync<T>(string key) where T : UnityEngine.Object;
    }
}