using System;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetHandle : IDisposable
    {
        public T GetResult<T>() where T : UnityEngine.Object;
    }
}