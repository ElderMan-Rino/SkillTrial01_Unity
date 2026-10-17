using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Infra
{
    public sealed class AssetCache : IAssetCache, IDependencyInjectable
    {
        private readonly Dictionary<string, IAssetHandle> _handleByKey = new();
        private readonly Dictionary<string, int> _refCountByKey = new();
        private readonly Dictionary<string, AsyncOperationHandle<IList<UnityEngine.Object>>> _handleByLabel = new();

        private ILoggerEx _logger = LogFacade.GetLogger<AssetCache>();
        private IAssetLoadService _loader;

        public Type ClassType => typeof(AssetCache);
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
            if (_handleByKey.TryGetValue(key, out var cached))
            {
                _refCountByKey[key]++;
                return cached;
            }

            var handle = await _loader.LoadAssetAsync<T>(key);
            _refCountByKey[key] = 1;
            return _handleByKey[key] = new AssetHandle<T>(handle, _loader, key, DecrementRef);
        }

        public void UnregisterKey(string key)
        {
            if (!_handleByKey.TryGetValue(key, out var handle)) return;

            handle.Dispose();
        }

        private bool DecrementRef(string key)
        {
            if (!_refCountByKey.TryGetValue(key, out var count)) return false;

            if (--count > 0)
            {
                _refCountByKey[key] = count;
                return false;
            }

            _refCountByKey.Remove(key);
            _handleByKey.Remove(key);
            return true;
        }

        // TODO(AssetSystem 캐시 이원화 수정 계획):
        // 1. IAssetCache에 GetAssetByLabelAsync(string label, string name) 추가.
        //    - GetAssetAsync<T>(key)는 라벨 없는 단일 에셋 조회 전용으로 유지.
        //    - name은 어드레서블 그룹 내 에셋 이름(=키)과 동일해야 한다는 규칙을 강제.
        // 2. _handleByKey를 (string Label, string Name) 복합키 Dictionary로 분리 추가 (예: _handleByLabeledKey).
        //    - 같은 name이라도 label이 다르면 별개 항목으로 취급해야 캐시 키 충돌을 피함.
        // 3. RegisterLabelAsync(label) 내부에서 라벨 로드 결과(handle.Result)를 순회하며
        //    _handleByLabeledKey[(label, asset.name)]를 채워서 개별 조회와 라벨 일괄 로드가 같은 캐시를 공유하게 함.
        // 4. GetAssetByLabelAsync(label, name)은:
        //    - _handleByLabeledKey에 캐시가 있으면 반환.
        //    - 없으면 RegisterLabelAsync(label)을 호출해 라벨 전체를 로드시킨 뒤 다시 조회 (A안: 자동 로드).
        //    - 그래도 없으면 실패 로그 후 null 반환.
        // 5. UnregisterLabel(label)에서 라벨 해제 시 _handleByLabeledKey에 남은 (label, *) 항목도 함께 제거.
        // 6. AssetHandle<T>는 개별 로드(AsyncOperationHandle<T>) 경로와 라벨 유입(개별 UnityEngine.Object) 경로를
        //    모두 감쌀 수 있어야 하므로 T result를 직접 받는 생성자 오버로드 필요.

        public async UniTask RegisterLabelAsync(string label)
        {
            if (_handleByLabel.ContainsKey(label)) return;

            var handle = await _loader.LoadAssetsAsync(label);
            _handleByLabel[label] = handle;
        }

        public void UnregisterLabel(string label)
        {
            if (!_handleByLabel.Remove(label, out var handle)) return;

            _loader.Release(handle);
        }
    }
}
