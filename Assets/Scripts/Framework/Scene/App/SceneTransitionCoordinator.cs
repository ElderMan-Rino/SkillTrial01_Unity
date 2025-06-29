//using Cysharp.Threading.Tasks;
//using Elder.Framework.Common.Base;
//using Elder.Framework.Common.Utils;
//using Elder.Framework.Flux.Interfaces;
//using Elder.Framework.Log.Helper;
//using Elder.Framework.Log.Interfaces;
//using Elder.Framework.Scene.Domain.Data;
//using Elder.Framework.Scene.Interfaces;
//using Elder.Framework.Scene.Messages;
//using VContainer.Unity;

//namespace Elder.Framework.Scene.App
//{
//    public class SceneTransitionCoordinator : DisposableBase, ISceneTransitionCoordinator, IInitializable
//    {
//        private readonly ISceneLoader _loader;
//        private readonly ISceneContextFactory _contextFactory;
//        private readonly IFluxRouter _router;
//        private ILoggerEx _logger;

//        private SceneLoadContext _currentContext;

//        public SceneTransitionCoordinator(ISceneLoader loader, ISceneContextFactory contextFactory, IFluxRouter router)
//        {
//            _loader = loader;
//            _contextFactory = contextFactory;
//            _router = router;
//        }

//        public void Initialize()
//        {
//            InitializeLogger();
//            SubscribeToFluxEvent();
//        }

//        private void InitializeLogger()
//        {
//            _logger = LogFacade.GetLoggerFor<SceneTransitionCoordinator>();
//        }

//        private void SubscribeToFluxEvent()
//        {
//            _router.Subscribe<FxSceneTransition>(HandleSceneTransition);
//        }

//        private void HandleSceneTransition(in FxSceneTransition fxMsg)
//        {
//            ProcessSceneTransitionFlowAsync(fxMsg.TargetSceneKey).Forget();
//        }

//        private async UniTask<bool> ProcessSceneTransitionFlowAsync(string targetSceneKey)
//        {
//            // 흐름 제어 로직이 들어갈 곳
//            var sceneHash = ComputeSceneHash(targetSceneKey);
//            if (!TryGetSceneMetaData(sceneHash))
//                return false;

//            var isAssetsPrepared = await PrepareSceneAssetsAsync();
//            if (!isAssetsPrepared)
//                return false;

//            var isSwitched = await SwitchToSceneAsync();
//            if (!isSwitched)
//                return false;

//            return true;
//        }

//        private int ComputeSceneHash(string sceneNameKey)
//        {
//            return StringHashHelper.ToStableHash(sceneNameKey);
//        }

//        private bool TryGetSceneMetaData(int sceneHash)
//        {
//            // 여기서 데이터 로드 인터페이스로 가져옴
//            // 여기서 다시 작업해야함
//            return true;
//        }

//        private async UniTask<bool> PrepareSceneAssetsAsync()
//        {
//            return true;
//        }

//        private async UniTask<bool> SwitchToSceneAsync()
//        {
//            return true;
//        }

//        private async UniTask<bool> LoadIntermediateSceneAsync()
//        {
//            return true;
//        }

//        private async UniTask<bool> CleanUpMemoryAsync()
//        {
//            return true;
//        }

//        private async UniTask<bool> LoadTargetSceneAsync()
//        {
//            return true;
//        }
//    }
//}