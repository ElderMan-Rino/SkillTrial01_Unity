using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Infra.Extensions;
using Elder.Framework.Common.Base;
using Elder.Framework.Common.Utils;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Flux.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.Scene.Domain.Models;
using Elder.Framework.Scene.Interfaces;
using Elder.Framework.Scene.Messages;
using Elder.SkillTrial.Scene.Domain;
using UnityEngine.ResourceManagement.ResourceProviders;
using VContainer.Unity;

namespace Elder.Framework.Scene.App
{
    internal sealed class SceneTransitionCoordinator : DisposableBase, ISceneTransitionCoordinator, IInitializable
    {
        private readonly ISceneLoader _loader;
        private readonly ISceneContextFactory _contextFactory;
        private readonly IFluxRouter _router;
        private readonly IDataProvider _dataProvider;
        private ILoggerEx _logger;

        private SubscriptionToken _sceneTransitionSubscription;
        private SceneTransitionState _state = SceneTransitionState.Idle;

        // 현재 로드된 씬 인스턴스 (Single/Additive 언로드 시 필요)
        private SceneInstance _activeSceneInstance;
        private bool _hasActiveScene;

        public SceneTransitionCoordinator(
            ISceneLoader loader,
            ISceneContextFactory contextFactory,
            IFluxRouter router,
            IDataProvider dataProvider)
        {
            _loader = loader;
            _contextFactory = contextFactory;
            _router = router;
            _dataProvider = dataProvider;
        }

        public void Initialize()
        {
            _logger = LogFacade.GetLoggerFor<SceneTransitionCoordinator>();
            _sceneTransitionSubscription = _router.Subscribe<FxSceneTransition>(HandleSceneTransition);
        }

        private void HandleSceneTransition(in FxSceneTransition fxMsg)
        {
            if (_state == SceneTransitionState.InProgress)
            {
                _logger.Warn($"씬 전환 중 중복 요청 무시: {fxMsg.TargetSceneKey}");  // [HEAP] 문자열 보간
                return;
            }

            ProcessSceneTransitionAsync(fxMsg.TargetSceneKey).Forget();
        }

        private async UniTaskVoid ProcessSceneTransitionAsync(string targetSceneKey)
        {
            _state = SceneTransitionState.InProgress;

            try
            {
                // BlobString은 async 경계를 넘길 수 없으므로 필요한 값을 먼저 string으로 복사
                if (!TryResolveSceneRow(targetSceneKey, out string addressableKey, out SceneLoadMode loadMode))
                    return;

                _router.Publish(new FxSceneTransitionStarted(targetSceneKey));

                var success = await ExecuteTransitionAsync(targetSceneKey, addressableKey, loadMode);
                if (!success)
                    _logger.Error($"씬 전환 실패: {targetSceneKey}");  // [HEAP] 문자열 보간
            }
            catch (System.Exception ex)
            {
                _logger.Error($"씬 전환 예외 [{targetSceneKey}]: {ex.Message}");  // [HEAP] 문자열 보간
            }
            finally
            {
                _state = SceneTransitionState.Idle;
            }
        }

        private async UniTask<bool> ExecuteTransitionAsync(string targetSceneKey, string addressableKey, SceneLoadMode loadMode)
        {
            if (loadMode == SceneLoadMode.AdditiveKeepPrevious)
                return await LoadAdditiveAsync(targetSceneKey, addressableKey);

            return await SwapWithTempAsync(targetSceneKey, addressableKey);
        }

        // Single / Additive: Empty를 중간 버퍼로 배치해 메모리 스파이크 방지
        // 순서: TempLoad → OldUnload → NewLoad → TempUnload
        private async UniTask<bool> SwapWithTempAsync(string targetSceneKey, string addressableKey)
        {
            // TempScene을 먼저 올려 이전 씬 언로드 시 활성 씬이 없는 상태 방지
            var tempInstance = await _loader.LoadSceneAsync(SceneConstants.TempSceneKey, UnityEngine.SceneManagement.LoadSceneMode.Additive, true);

            if (_hasActiveScene)
                await _loader.UnloadSceneAsync(_activeSceneInstance);

            var context = _contextFactory.Create(targetSceneKey);
            // Single/Additive 모두 내부적으로 Additive 로드 후 TempScene 언로드 — Unity의 Single은 DontDestroyOnLoad를 제거하므로 사용 안 함
            var newInstance = await _loader.LoadSceneAsync(addressableKey, UnityEngine.SceneManagement.LoadSceneMode.Additive, true);

            await _loader.UnloadSceneAsync(tempInstance);

            _activeSceneInstance = newInstance;
            _hasActiveScene = true;
            _contextFactory.Release(context);

            _router.Publish(new FxSceneTransitionCompleted(targetSceneKey));
            return true;
        }

        // AdditiveKeepPrevious: 이전 씬 유지, 신규 씬만 추가
        private async UniTask<bool> LoadAdditiveAsync(string targetSceneKey, string addressableKey)
        {
            var context = _contextFactory.Create(targetSceneKey);
            var newInstance = await _loader.LoadSceneAsync(addressableKey, UnityEngine.SceneManagement.LoadSceneMode.Additive, true);

            _activeSceneInstance = newInstance;
            _hasActiveScene = true;
            _contextFactory.Release(context);

            _router.Publish(new FxSceneTransitionCompleted(targetSceneKey));
            return true;
        }

        // BlobAsset에서 필요한 값만 string으로 복사해 반환. ref 반환 불가(async 경계) 대안.
        private bool TryResolveSceneRow(string targetSceneKey, out string addressableKey, out SceneLoadMode loadMode)
        {
            addressableKey = null;
            loadMode = default;

            var blobRef = _dataProvider.GetBlobReference<SceneInfoRoot>();
            ref var table = ref blobRef.Value;

            int targetHash = StringHashHelper.ToStableHash(targetSceneKey);

            for (int i = 0; i < table.Rows.Length; i++)
            {
                ref var candidate = ref table.Rows[i];
                // [HEAP] BlobString.ToString() — 공개 API 제약으로 힙 불가피. 씬 수만큼 발생.
                if (StringHashHelper.ToStableHash(candidate.Key.ToString()) == targetHash)
                {
                    // [HEAP] BlobString.ToString() — async 경계 통과를 위한 불가피한 string 복사
                    addressableKey = candidate.AddressableKey.ToString();
                    loadMode = candidate.LoadMode;
                    return true;
                }
            }

            _logger.Warn($"SceneRow not found for key: {targetSceneKey}");  // [HEAP] 문자열 보간
            return false;
        }

        protected override void DisposeManagedResources()
        {
            _sceneTransitionSubscription.Dispose();
            base.DisposeManagedResources();
        }
    }
}
