using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetRegistry : IGameSystem
    {
        public UniTask RegisterAsync(string label);
        public void Unregister(string label);
        public T Get<T>(string label, string assetName) where T : UnityEngine.Object;
    }
}
