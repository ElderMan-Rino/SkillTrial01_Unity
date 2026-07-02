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
        private readonly HashSet<string> _pendingUnloads = new();

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IEngineAssetLoader>(out _loader))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IEngineAssetLoader)}");
            if (!TryGetSystem<IEngineAssetReleaser>(out _releaser))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IEngineAssetReleaser)}");
        }

        public override UniTask InitializeAsync() => UniTask.CompletedTask;
        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;

        public async UniTask RegisterAsync<T>(string label) where T : UnityEngine.Object
        {
            if (_entries.ContainsKey(label)) return;
            await LoadLabelAsync<T>(label);
        }

        public void Unregister(string label)
        {
            if (_pendingLoads.ContainsKey(label))
            {
                _pendingUnloads.Add(label);
                return;
            }

            ReleaseLabelEntry(label);
        }

        public T Get<T>(string label, string assetName) where T : UnityEngine.Object
        {
            if (!_entries.TryGetValue(label, out var entry))
                throw new InvalidOperationException(
                    $"Label '{label}' is not registered. Call RegisterAsync first.");

            if (!entry.TryGetAsset(typeof(T), assetName, out var asset))
                throw new InvalidOperationException(
                    $"Asset '{assetName}' of type '{typeof(T).Name}' not found in label '{label}'.");

            return (T)asset;
        }

        private async UniTask LoadLabelAsync<T>(string label) where T : UnityEngine.Object
        {
            if (_entries.ContainsKey(label)) return;

            if (_pendingLoads.TryGetValue(label, out var pending))
            {
                await pending;
                return;
            }

            var loadTask = ExecuteLoadAsync<T>(label);
            _pendingLoads[label] = loadTask;

            try
            {
                await loadTask;
            }
            finally
            {
                _pendingLoads.Remove(label);

                if (_pendingUnloads.Remove(label))
                    ReleaseLabelEntry(label);
            }
        }

        private async UniTask ExecuteLoadAsync<T>(string label) where T : UnityEngine.Object
        {
            var handle = await _loader.LoadAllAsync<T>(label);
            var entry = new LabelEntry { Handle = handle };

            var results = handle.Result;
            for (int i = 0; i < results.Count; i++)
                entry.AddAsset(typeof(T), results[i].name, results[i]);

            _entries[label] = entry;
        }

        private void ReleaseLabelEntry(string label)
        {
            if (!_entries.TryGetValue(label, out var entry)) return;
            _releaser.Release(entry.Handle);
            _entries.Remove(label);
        }

        public override void PreDispose() { }

        protected override void DisposeManagedResources()
        {
            foreach (var entry in _entries)  // [HEAP] Dictionary 열거자 할당
                _releaser.Release(entry.Value.Handle);
            _entries.Clear();
            _loader = null;
            _releaser = null;
        }
    }
}
