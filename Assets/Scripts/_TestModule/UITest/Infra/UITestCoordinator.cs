using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.UI.Definitions;
using Elder.Framework.UI.Infra;
using Elder.Framework.UI.Interfaces;
using Elder.TestModule.Common.Definitions;
using Elder.TestModule.UITest.App;
using Elder.TestModule.UITest.Interfaces;
using System;

namespace Elder.TestModule.UITest.Infra
{

    public class UITestCoordinator : IUITestCoordinator, IDependencyInjectable, IPreDisposable, IDisposable
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UITestCoordinator>();

        private IUISystem _uiSystem;
        private UISystemTestView _uiSystemTestView;
        private IUIRebindStrategy _childContainerStrategy;
        private bool _isPanelOpening;
        private bool _isPopupOpening;

        public Type ClassType => typeof(UITestCoordinator);

        public int Scope => TestScope.Id;


        public void InjectDependency(IComponentProvider provider)
        {
            if (!provider.TryGetComponent<IUISystem>(out _uiSystem))
                _logger.Error($"Failed to resolve {nameof(IUISystem)}."); // [HEAP]
        }

        // MVP+VM 연동 플랜 (TestModule UI):
        // UITestFlowHandler.HandleModuleStart -> StartAsync() 진입
        //   -> IUISystem.TryFindUIView<TView>()로 씬에 이미 존재하는 View(MonoBehaviour)를 탐색
        //      (Addressable 동적 로드가 아니라 씬에 배치된 View를 그대로 사용하는 경로이므로 IUIViewFactory는 사용하지 않는다)
        //   -> 탐색 성공 시 IUIPresenterFactory.Create<TPresenter>()로 Presenter 생성 (new + InjectDependency)
        //   -> presenter.BindView(view) 호출로 View의 Observable <-> Presenter 핸들러 연결 (AssetLoadTestPresenter.BindView 참고)
        //   -> Presenter 내부에서 GetOrCreateViewModel<TViewModel>(factory)로 VM 생성/재사용 (Sleep 중에도 Presenter와 함께 유지)
        // 의존 방향: UITestFlowHandler -> UITestCoordinator -> IUISystem/IUIPresenterFactory (역방향 없음, Coordinator는 Flux를 모름)
        public UniTask StartAsync()
        {
            SettingModulePanel();

            return UniTask.CompletedTask;
        }

        private void SettingModulePanel()
        {
            if (!_uiSystem.TryFindUIView<UISystemTestView>(out var uiView))
                return;

            _uiSystemTestView = uiView;
            _childContainerStrategy = new ChildContainerRebindStrategy(_uiSystemTestView.ChildContainer); // [HEAP]

            var presenter = _uiSystem.CreatePresenter<UISystemTestPresenter>();
            presenter.BindView(uiView);
        }

        public async UniTask OpenTestPanelAsync()
        {
            if (_isPanelOpening)
                return;

            _isPanelOpening = true;

            try
            {
                var viewHandle = await _uiSystem.CreateViewByAssetKeyAsync<UITestPanelView>("UITestPanel");
                if (viewHandle is null)
                {
                    _logger.Error("Failed to create UITestPanelView."); // [HEAP]
                    return;
                }

                viewHandle.View.BindRoot(_uiSystemTestView.ChildContainer);

                var presenter = _uiSystem.CreatePresenter<UITestPanelPresenter>();
                presenter.BindView(viewHandle.View);

                var entry = _uiSystem.CreateEntry(TestScope.Id, viewHandle, presenter, _childContainerStrategy);
                await _uiSystem.OpenSceneAsync(entry);
            }
            finally
            {
                _isPanelOpening = false;
            }
        }

        public async UniTask OpenTestPopupAsync()
        {
            if (_isPopupOpening)
                return;

            _isPopupOpening = true;

            try
            {
                var viewHandle = await _uiSystem.CreateViewByAssetKeyAsync<UITestPopupView>("UITestPopup");
                if (viewHandle is null)
                {
                    _logger.Error("Failed to create UITestPopupView."); // [HEAP]
                    return;
                }

                viewHandle.View.BindRoot(_uiSystemTestView.ChildContainer);

                var presenter = _uiSystem.CreatePresenter<UITestPopupPresenter>();
                presenter.BindView(viewHandle.View);

                var entry = _uiSystem.CreateEntry(TestScope.Id, viewHandle, presenter, _childContainerStrategy);
                await _uiSystem.OpenSceneAsync(entry);
            }
            finally
            {
                _isPopupOpening = false;
            }
        }

        public void PreDispose()
        {
            _uiSystem.ClearSceneScopeAsync(TestScope.Id).Forget();
            _uiSystem = null;
            _uiSystemTestView = null;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}