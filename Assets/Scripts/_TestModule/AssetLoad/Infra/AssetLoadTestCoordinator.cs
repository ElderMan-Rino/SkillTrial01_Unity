using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Signal.Messages;
using Elder.TestModule.AssetLoad.App;
using Elder.TestModule.AssetLoad.Interfaces;
using Elder.TestModule.Common.Definitions;
using System;

namespace Elder.TestModule.AssetLoad.Infra
{
    internal sealed class AssetLoadTestCoordinator : IAssetLoadTestCoordinator, IDependencyInjectable, IPreDisposable, IDisposable
    {
        private const string AssetLoadTestKey = "AssetLoadTest";
        private ILoggerEx _logger = LogFacade.GetLogger<AssetLoadTestCoordinator>();
        private IUIViewFactory _viewFactory;
        private IUIPresenterFactory _presenterFactory;
        private SignalToken _token;
        private IUIPresenter _assetLoadTestPresenter;
        private IUIViewHandle _assetLoadTestViewHandle;

        public Type ClassType => typeof(AssetLoadTestCoordinator);
        public int Scope => TestScope.Id;

        public void InjectDependency(IComponentProvider provider)
        {
            SubscribeToSignal(provider);
            //RegistViewCreator(provider);
            //RegistPresenterCreator(provider);
        }

        //private void RegistPresenterCreator(IComponentProvider provider)
        //{
        //    if (!provider.TryGetComponent<IUIPresenterFactory>(out _presenterFactory))
        //        throw new InvalidOperationException($"Failed to resolve {nameof(IUIPresenterCreatorRegistry)}."); // [HEAP]

        //    _presenterFactory.Register<AssetLoadTestPresenter>(AssetLoadTestKey, CreateAssetLoadTestPresenter); // [HEAP]
        //}

        //private IUIPresenter CreateAssetLoadTestPresenter(IComponentProvider provider)
        //{
        //    var presenter = new AssetLoadTestPresenter(); // [HEAP]
        //    presenter.InjectDependency(provider);
        //    return presenter;
        //}

        //private void RegistViewCreator(IComponentProvider provider)
        //{
        //    if (!provider.TryGetComponent<IUIViewFactory>(out _viewFactory))
        //        throw new InvalidOperationException($"Failed to resolve {nameof(IUIViewCreatorRegistry)}."); // [HEAP]

        //    _viewFactory.Register<AssetLoadTestView>(AssetLoadTestKey, CreateAssetLoadTestView);
        //}

        //private UniTask<IUIViewHandle> CreateAssetLoadTestView()
        //{
        //    var assetLoadTestView = UnityEngine.Object.FindAnyObjectByType<AssetLoadTestView>();
        //    if (assetLoadTestView == null)
        //        return UniTask.FromException<IUIViewHandle>(new InvalidOperationException($"Failed to find {nameof(AssetLoadTestView)} in scene.")); // [HEAP]

        //    return UniTask.FromResult<IUIViewHandle>(new SceneResidentHandle(assetLoadTestView)); // [HEAP]
        //}

        private void SubscribeToSignal(IComponentProvider provider)
        {
            if (!provider.TryGetComponent<ISignalSubscriber>(out var subscriber))
                throw new InvalidOperationException($"Failed to resolve {nameof(ISignalSubscriber)}."); // [HEAP]

            _token = subscriber.Subscribe<FxModuleStartSignal>(HandleModuleStart);
        }

        private void HandleModuleStart(in FxModuleStartSignal signal)
        {
            if (signal.ModuleType != typeof(AssetLoadTestCoordinator))
                return;

            SettingAssetTestUIAsync().Forget();
        }

        private async UniTask SettingAssetTestUIAsync()
        {
            try
            {
                //_assetLoadTestViewHandle = await _viewFactory.CreateAsync(AssetLoadTestKey);
                //_assetLoadTestPresenter = _presenterFactory.Create(AssetLoadTestKey);
                //_assetLoadTestPresenter.BindView(_assetLoadTestViewHandle.View);
                await UniTask.CompletedTask;
            }
            catch (Exception exception)
            {
                _logger.Error($"Failed to setup {nameof(AssetLoadTestView)}: {exception.Message}"); // [HEAP]
            }
        }

        public void PreDispose()
        {
            _assetLoadTestPresenter?.Dispose();
            _assetLoadTestPresenter = null;

            _assetLoadTestViewHandle?.Dispose();
            _assetLoadTestViewHandle = null;

            _token.Dispose();
        }

        public void Dispose()
        {
            //UnregistPresenterCreator();
            //UnregistViewCreator();

            _viewFactory = null;
            _presenterFactory = null;
            _logger = null;
        }

        //private void UnregistPresenterCreator()
        //{
        //    _presenterFactory.Unregister(AssetLoadTestKey);
        //}

        //private void UnregistViewCreator()
        //{
        //    _viewFactory.Unregister(AssetLoadTestKey);
        //}
    }
}
