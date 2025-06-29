using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Infra.Extensions;
using Elder.Framework.Common.Base;
using Elder.Framework.Common.Utils;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.Scene.Domain.Models;
using Elder.Framework.Scene.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.SkillTrial.Resources.Data;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Scene.App
{
    internal sealed class SceneTransitionExecutor : DisposableBase, ISceneTransitionExecutor
    {
        private readonly ISceneLoader _loader;
        private readonly ISceneContextFactory _contextFactory;
        private readonly IDataProvider _dataProvider;
        private readonly ILoggerEx _logger;

        private SceneInstance _activeSceneInstance;
        private bool _hasActiveScene;

        public SceneTransitionExecutor(ISceneLoader loader, ISceneContextFactory contextFactory, IDataProvider dataProvider, ILoggerPublisher loggerPublisher)
        {
            _loader = loader;
            _contextFactory = contextFactory;
            _dataProvider = dataProvider;
            _logger = loggerPublisher.GetLogger<SceneTransitionExecutor>();
        }

        public async UniTask<bool> ExecuteAsync(string targetSceneKey)
        {
            if (!TryResolveSceneRow(targetSceneKey, out string addressableKey, out SceneLoadType loadType))
                return false;

            if (loadType == SceneLoadType.Additive)
                return await LoadAdditiveAsync(targetSceneKey, addressableKey);

            return await SwapWithTempAsync(targetSceneKey, addressableKey);
        }

        // Single / Additive: Empty를 중간 버퍼로 배치해 메모리 스파이크 방지
        // 순서: TempLoad → OldUnload → NewLoad → TempUnload
        private async UniTask<bool> SwapWithTempAsync(string targetSceneKey, string addressableKey)
        {
            var tempInstance = await _loader.LoadSceneAsync(SceneConstants.TempSceneKey, LoadSceneMode.Additive, true);

            if (_hasActiveScene)
                await _loader.UnloadSceneAsync(_activeSceneInstance);

            var context = _contextFactory.Create(targetSceneKey);
            var newInstance = await _loader.LoadSceneAsync(addressableKey, LoadSceneMode.Additive, true);

            await _loader.UnloadSceneAsync(tempInstance);

            _activeSceneInstance = newInstance;
            _hasActiveScene = true;
            _contextFactory.Release(context);
            return true;
        }

        private async UniTask<bool> LoadAdditiveAsync(string targetSceneKey, string addressableKey)
        {
            var context = _contextFactory.Create(targetSceneKey);
            var newInstance = await _loader.LoadSceneAsync(addressableKey, LoadSceneMode.Additive, true);

            _activeSceneInstance = newInstance;
            _hasActiveScene = true;
            _contextFactory.Release(context);
            return true;
        }

        // BlobAsset에서 필요한 값만 string으로 복사해 반환. ref 반환 불가(async 경계) 대안.
        private bool TryResolveSceneRow(string targetSceneKey, out string addressableKey, out SceneLoadType loadType)
        {
            addressableKey = null;
            loadType = default;

            var blobRef = _dataProvider.GetBlobReference<SceneInfoRoot>();
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

            _logger.Warn($"SceneRow not found for key: {targetSceneKey}");  // [HEAP] 문자열 보간
            return false;
        }
    }
}
