using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetRegistry : IGameSystem
    {
        public UniTask RegisterAsync<T>(string label) where T : UnityEngine.Object;
        public void Unregister(string label);
        public T Get<T>(string label, string assetName) where T : UnityEngine.Object;
    }
}
