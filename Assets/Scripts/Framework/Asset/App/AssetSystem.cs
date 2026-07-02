using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core;
using System.Collections.Generic;

namespace Elder.Framework.Asset.App
{
    internal sealed class AssetSystem : BaseSystem, IAssetProvider
    {
        private readonly Dictionary<string, ProviderEntry> _entries = new();
        
        private IEngineAssetLoader _loader;
        private IEngineAssetReleaser _releaser;


        protected override void HandleInjectDependency()
        {
            TryGetSystem<IEngineAssetLoader>(out _loader);
            TryGetSystem<IEngineAssetReleaser>(out _releaser);
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
            DisposeEntries();
        }

        private void DisposeEntries()
        {
            foreach (var entry in _entries)  
                _releaser.Release(entry.Value.Handle);
            _entries.Clear();
        }

        public override UniTask InitializeAsync() => UniTask.CompletedTask;

        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;
    }
}