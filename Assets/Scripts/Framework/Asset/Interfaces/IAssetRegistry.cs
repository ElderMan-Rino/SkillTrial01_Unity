using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetRegistry : IGameSystem, IDisposable
    {
        public UniTask PreloadAsync(string label);
        public UniTask<T> GetAssetAsync<T>(string label, string assetName) where T : UnityEngine.Object;
        public void Unload(string label);
    }
}