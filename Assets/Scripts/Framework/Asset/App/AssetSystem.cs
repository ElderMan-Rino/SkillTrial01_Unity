using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Common.Base;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.App
{
    internal sealed class AssetSystem : DisposableBase, IAssetProvider
    {
        private readonly IEngineAssetLoader _loader;
        private readonly IEngineAssetReleaser _releaser;
        private readonly Dictionary<string, ProviderEntry> _entries = new();

        public AssetSystem(IEngineAssetLoader loader, IEngineAssetReleaser releaser)
        {
            _loader = loader;
            _releaser = releaser;
        }

        public async UniTask<IAssetHandle<T>> GetAssetAsync<T>(string key)
            where T : UnityEngine.Object
        {
            if (!_entries.TryGetValue(key, out var entry))
            {
                var handle = await _loader.LoadAsync<T>(key);
                entry = new ProviderEntry
                {
                    Handle = handle,
                    Asset = handle.Result,
                    RefCount = 0
                };
                _entries[key] = entry;
            }

            entry.RefCount++;
            return new AssetHandle<T>((T)entry.Asset, () => Release(key));
        }

        private void Release(string key)
        {
            if (!_entries.TryGetValue(key, out var entry))
                return;

            entry.RefCount--;

            if (entry.RefCount > 0)
                return;

            _releaser.Release(entry.Handle);
            _entries.Remove(key);
        }

        protected override void DisposeManagedResources()
        {
            foreach (var entry in _entries.Values)
                _releaser.Release(entry.Handle);
            _entries.Clear();
        }
    }
}