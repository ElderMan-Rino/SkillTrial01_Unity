using System;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetHandle<out T> : IDisposable where T : UnityEngine.Object
    {
        public T Asset { get; }
    }
}