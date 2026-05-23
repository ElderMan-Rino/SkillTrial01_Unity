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
                // [HEAP] 키당 최초 로드 시 ProviderEntry + ReleaseAction 1회 할당
                entry = new ProviderEntry
                {
                    Handle = handle,
                    Asset = handle.Result,
                    RefCount = 0
                };
                entry.ReleaseAction = () => Release(key);
                _entries[key] = entry;
            }

            entry.RefCount++;
            return new AssetHandle<T>((T)entry.Asset, entry.ReleaseAction);
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
            foreach (var entry in _entries.Values)  // [HEAP] Dictionary.Values 열거자 할당
                _releaser.Release(entry.Handle);
            _entries.Clear();
        }
    }
}