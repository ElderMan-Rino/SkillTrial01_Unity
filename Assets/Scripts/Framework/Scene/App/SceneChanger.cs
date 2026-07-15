using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Infra.Extensions;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Common.Utils;
using Elder.Framework.Core;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.Scene.Interfaces;
using Elder.SkillTrial.Resources.Data;
using System;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Scene.App
{
    internal sealed class SceneChanger : BaseSystem, ISceneChanger
    {
        private ISceneLoader _loader;
        private ISceneContextFactory _contextFactory;
        private IDataProvider _dataProvider;
        private ILoggerEx _logger;

        private SceneInstance _activeSceneInstance;
        private bool _hasActiveScene;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<ISceneLoader>(out _loader))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISceneLoader)}");
            if (!TryGetSystem<ISceneContextFactory>(out _contextFactory))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISceneContextFactory)}");
            if (!TryGetSystem<IDataProvider>(out _dataProvider))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IDataProvider)}");
            if (TryGetSystem<ILoggerPublisher>(out var loggerPublisher)) _logger = loggerPublisher.GetLogger<SceneChanger>();
        }


        public async UniTask<bool> ExecuteAsync(string targetSceneKey)
        {
            if (!TryResolveSceneRow(targetSceneKey, out string addressableKey, out SceneLoadType loadType)) return false;

            if (loadType == SceneLoadType.Additive) return await LoadAdditiveAsync(targetSceneKey, addressableKey);

            return await SwapWithTempAsync(targetSceneKey, addressableKey);
        }

        // Single / Additive: Empty를 중간 버퍼로 배치해 메모리 스파이크 방지
        // 순서: TempLoad → OldUnload → NewLoad → TempUnload
        private async UniTask<bool> SwapWithTempAsync(string targetSceneKey, string addressableKey)
        {
            var builtinScene = _hasActiveScene ? default : SceneManager.GetActiveScene();
            var tempInstance = await _loader.LoadSceneAsync(SceneConstants.EmptySceneKey, LoadSceneMode.Additive, true);

            if (_hasActiveScene)
                await _loader.UnloadSceneAsync(_activeSceneInstance);
            else
                await SceneManager.UnloadSceneAsync(builtinScene);

            var context = _contextFactory.Create(targetSceneKey);
            try
            {
                var newInstance = await _loader.LoadSceneAsync(addressableKey, LoadSceneMode.Additive, true);

                await _loader.UnloadSceneAsync(tempInstance);

                _activeSceneInstance = newInstance;
                _hasActiveScene = true;
                return true;
            }
            finally
            {
                _contextFactory.Release(context);
            }
        }

        private async UniTask<bool> LoadAdditiveAsync(string targetSceneKey, string addressableKey)
        {
            var context = _contextFactory.Create(targetSceneKey);
            try
            {
                var newInstance = await _loader.LoadSceneAsync(addressableKey, LoadSceneMode.Additive, true);

                _activeSceneInstance = newInstance;
                _hasActiveScene = true;
                return true;
            }
            finally
            {
                _contextFactory.Release(context);
            }
        }

        // BlobAsset에서 필요한 값만 string으로 복사해 반환. ref 반환 불가(async 경계) 대안.
        // SceneInfoRoot Blob 미로드 시 씬 키 자체를 어드레서블 키로 fallback (Splash 등 부트 초기 씬 대응).
        private bool TryResolveSceneRow(string targetSceneKey, out string addressableKey, out SceneLoadType loadType)
        {
            addressableKey = null;
            loadType = default;

            if (!_dataProvider.TryGetBlobReference<SceneInfoRoot>(out var blobRef))
            {
                addressableKey = targetSceneKey;
                loadType = SceneLoadType.Single;
                return true;
            }

            ref var table = ref blobRef.Value;

            int targetHash = StringHashHelper.ToStableHash(targetSceneKey);

            for (int i = 0; i < table.Rows.Length; i++)
            {
                ref var candidate = ref table.Rows[i];
                if (StringHashHelper.ToStableHash(ref candidate.Key) == targetHash)
                {
                    // [HEAP] BlobString.ToString() — async 경계 통과를 위한 불가피한 string 복사
                    addressableKey = candidate.SceneKey.ToString();
                    loadType = candidate.LoadMode;
                    return true;
                }
            }

            _logger?.Warn($"SceneRow not found for key: {targetSceneKey}");  // [HEAP] 문자열 보간
            return false;
        }


        public override UniTask InitializeAsync() => UniTask.CompletedTask;

        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;

        protected override void DisposeManagedResources()
        {
            _loader = null;
            _contextFactory = null;
            _dataProvider = null;
            _logger = null;
        }
    }
}
