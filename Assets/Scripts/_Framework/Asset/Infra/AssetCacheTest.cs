using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Definitions;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Infra
{
    public sealed class AssetCacheTest : IAssetCacheTest, IDependencyInjectable
    {
        private readonly Dictionary<string, IAssetHandle> _handleByKey = new();
        private readonly Dictionary<string, AsyncOperationHandle<IList<UnityEngine.Object>>> _handleByLabel = new();

        private ILoggerEx _logger = LogFacade.GetLogger<AssetCacheTest>();
        private IAssetLoadService _loader;
        private UniTask? _registerAllKeysTask;

        public Type ClassType => typeof(AssetCacheTest);
        public int Scope => GlobalScope.Id;

        public void InjectDependency(IComponentProvider provider)
        {
            if (!provider.TryGetComponent<IAssetLoadService>(out _loader))
            {
                _logger.Error($"Failed to resolve {nameof(IAssetLoadService)}."); // [HEAP]
                return;
            }
        }

        public async UniTask<IAssetHandle> GetAssetAsync<T>(string key) where T : UnityEngine.Object
        {
            if (_handleByKey.TryGetValue(key, out var cached)) return cached;

            var handle = await _loader.LoadAssetAsync<T>(key);
            return _handleByKey[key] = new AssetHandle<T>(handle, _loader, key, _ => true);
        }

        public async UniTask RegisterLabelAsync(string label)
        {
            if (_handleByLabel.ContainsKey(label)) return;

            await _loader.EnsureInitializedAsync();
            var handle = await _loader.LoadAssetsAsync(label);
            _handleByLabel[label] = handle;
        }

        public UniTask RegisterAllKeysAsync()
        {
            if (_registerAllKeysTask is { } running) return running;

            var task = RegisterAllKeysCoreAsync();
            _registerAllKeysTask = task;
            return task;
        }

        private async UniTask RegisterAllKeysCoreAsync()
        {
            try
            {
                await _loader.EnsureInitializedAsync();
                foreach (var key in _loader.GetAllKeys())
                {
                    if (_handleByKey.ContainsKey(key)) continue;

                    if (!await _loader.IsLoadableAsync<UnityEngine.Object>(key))
                    {
                        if (await _loader.IsSceneLoadableAsync(key))
                        {
                            _logger.Info($"Skip scene key '{key}' (loadable via Addressables.LoadSceneAsync)."); // [HEAP]
                        }
                        continue;
                    }

                    try
                    {
                        await GetAssetAsync<UnityEngine.Object>(key);
                    }
                    catch (Exception exception)
                    {
                        _logger.Warn($"Skip non-address key '{key}': {exception.Message}"); // [HEAP]
                    }
                }
            }
            finally
            {
                _registerAllKeysTask = null;
            }
        }

        public void UnregisterKey(string key)
        {
            if (!_handleByKey.Remove(key, out var handle)) return;

            handle.Dispose();
        }

        public void UnregisterLabel(string label)
        {
            if (!_handleByLabel.Remove(label, out var handle)) return;

            _loader.Release(handle);
        }

        public async UniTask ReleaseAllAsync()
        {
            foreach (var handle in _handleByKey.Values)
            {
                handle.Dispose();
            }
            _handleByKey.Clear();

            foreach (var handle in _handleByLabel.Values)
            {
                _loader.Release(handle);
            }
            _handleByLabel.Clear();

            await Resources.UnloadUnusedAssets().ToUniTask();
            GC.Collect();
        }

        public IReadOnlyCollection<string> GetCachedKeys()
        {
            return _handleByKey.Keys;
        }

        public IReadOnlyCollection<string> GetRegisteredLabels()
        {
            return _handleByLabel.Keys;
        }

        public AssetCacheMemoryStatus GetMemoryStatus()
        {
            return new AssetCacheMemoryStatus(_handleByKey.Count, _handleByLabel.Count, Profiler.GetTotalAllocatedMemoryLong());
        }
    }
}
