using System;
using System.Collections.Generic;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetBatchHandle<out T> : IDisposable where T : UnityEngine.Object
    {
        public IReadOnlyList<T> Assets { get; }
    }
}