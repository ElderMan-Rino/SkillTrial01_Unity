using Elder.Framework.Core;
using Elder.Framework.UI.Interfaces;
using Elder.Framework.UI.Loading.App;
using Elder.Framework.UI.Loading.Definitions;
using Elder.Framework.UI.Loading.Interfaces;

namespace Elder.Framework.UI.App
{
    internal sealed class UISystem : BaseSystem, IUISystem
    {
        private ILoadingView _loadingView;
        private LoadingPresenter _loadingPresenter;

        // FrameworkRoot 등 MonoBehaviour 쪽에서 주입
        public void InjectLoadingView(ILoadingView view)
        {
            _loadingView = view;
        }

        public ILoadingReporter ShowLoading(bool showBackground = true, LoadingVisualConfig visualConfig = default)
        {
            HideLoading();

            var model = new SceneTransitionLoadingModel(); // [HEAP] Model 1회 할당
            var vm = new LoadingViewModel();               // [HEAP] VM 1회 할당
            _loadingPresenter = new LoadingPresenter(model, vm, _loadingView); // [HEAP] Presenter 1회 할당

            _loadingView.SetGaugeType(visualConfig.GaugeType);

            if (visualConfig.VideoPlayer is not null)
                _loadingView.SetVideoPlayer(visualConfig.VideoPlayer);

            if (showBackground)
            {
                _loadingView.SetBackgroundImage(visualConfig.BackgroundSprite);
                _loadingView.Show();
            }
            else
            {
                _loadingView.ShowProgressOnly();
            }

            return model;
        }

        public void HideLoading()
        {
            if (_loadingPresenter is null) return;
            _loadingPresenter.Dispose();
            _loadingPresenter = null;
            _loadingView?.Hide();
        }

        public void TickLoading(float deltaTime)
        {
            _loadingPresenter?.Tick(deltaTime);
        }

        protected override void OnDispose()
        {
            HideLoading();
        }
    }
}
