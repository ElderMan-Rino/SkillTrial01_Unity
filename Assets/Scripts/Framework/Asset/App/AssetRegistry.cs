using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Asset.App
{
    internal sealed class AssetRegistry : BaseSystem, IAssetRegistry
    {
        private IEngineAssetLoader _loader;
        private IEngineAssetReleaser _releaser;
        private readonly Dictionary<string, LabelEntry> _entries = new();
        private readonly Dictionary<string, UniTask> _pendingLoads = new();

        protected override bool OnInjectDependency()
        {
            TryGetSystem<IEngineAssetLoader>(out _loader);
            TryGetSystem<IEngineAssetReleaser>(out _releaser);
            return true;
        }

        public async UniTask PreloadAsync(string label)
        {
            if (_entries.ContainsKey(label))
                return;

            await LoadLabelAsync(label);
        }

        public async UniTask<T> GetAssetAsync<T>(string label, string assetName)
            where T : UnityEngine.Object
        {
            if (!_entries.ContainsKey(label))
                await LoadLabelAsync(label);

            var entry = _entries[label];

            if (!entry.TryGetAsset(typeof(T), assetName, out var asset))
                throw new InvalidOperationException(
                    $"Asset '{assetName}' of type '{typeof(T).Name}' " +
                    $"not found in label '{label}'");

            return (T)asset;
        }

        public void Unload(string label)
        {
            if (_pendingLoads.ContainsKey(label))
                return;

            if (!_entries.TryGetValue(label, out var entry))
                return;

            _releaser.Release(entry.Handle);
            _entries.Remove(label);
        }

        private async UniTask LoadLabelAsync(string label)
        {
            if (_pendingLoads.TryGetValue(label, out var pending))
            {
                await pending;
                return;
            }

            var loadTask = ExecuteLoadAsync(label);
            _pendingLoads[label] = loadTask;

            try
            {
                await loadTask;
            }
            finally
            {
                _pendingLoads.Remove(label);
            }
        }

        private async UniTask ExecuteLoadAsync(string label)
        {
            var handle = await _loader.LoadAllAsync(label);
            var entry = new LabelEntry { Handle = handle };

            var results = handle.Result;
            for (int i = 0; i < results.Count; i++)
                entry.AddAsset(results[i].GetType(), results[i].name, results[i]);

            _entries[label] = entry;
        }

        protected override void OnDispose()
        {
            foreach (var entry in _entries.Values)  // [HEAP] Dictionary.Values 열거자 할당
                _releaser.Release(entry.Handle);
            _entries.Clear();
        }
    }
}