using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Infra.Extensions;
using Elder.Framework.Common.Base;
using Elder.Framework.Common.Utils;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Flux.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Scene.Domain.Data;
using Elder.Framework.Scene.Interfaces;
using Elder.Framework.Scene.Messages;
using Elder.SkillTrial.Scene.Domain;
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

        private SceneLoadContext _currentContext;
        private SubscriptionToken _sceneTransitionSubscription;

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
            ProcessSceneTransitionFlowAsync(fxMsg.TargetSceneKey).Forget();
        }

        private async UniTask<bool> ProcessSceneTransitionFlowAsync(string targetSceneKey)
        {
            // BlobString은 async 경계를 넘길 수 없으므로 필요한 값을 먼저 string으로 복사
            if (!TryResolveSceneKeys(targetSceneKey, out string addressableKey, out SceneLoadMode loadMode))
                return false;

            var isAssetsPrepared = await PrepareSceneAssetsAsync();
            if (!isAssetsPrepared)
                return false;

            var isSwitched = await SwitchToSceneAsync();
            if (!isSwitched)
                return false;

            return true;
        }

        // BlobAsset에서 필요한 값만 string으로 복사해 반환. ref 반환 불가(async 경계) 대안.
        private bool TryResolveSceneKeys(string targetSceneKey, out string addressableKey, out SceneLoadMode loadMode)
        {
            addressableKey = null;
            loadMode = default;

            var blobRef = _dataProvider.GetBlobReference<SceneTableRoot>();
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

        private async UniTask<bool> PrepareSceneAssetsAsync()
        {
            return true;
        }

        private async UniTask<bool> SwitchToSceneAsync()
        {
            return true;
        }

        protected override void DisposeManagedResources()
        {
            _sceneTransitionSubscription.Dispose();
            base.DisposeManagedResources();
        }
    }
}
