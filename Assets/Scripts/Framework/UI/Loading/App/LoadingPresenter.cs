using Elder.Framework.Common.Base;
using Elder.Framework.UI.Loading.Interfaces;
using UnityEngine;

namespace Elder.Framework.UI.Loading.App
{
    internal sealed class LoadingPresenter : DisposableBase
    {
        private readonly ILoadingModel _model;
        private readonly LoadingViewModel _viewModel;
        private readonly ILoadingView _view;

        public LoadingPresenter(ILoadingModel model, LoadingViewModel viewModel, ILoadingView view)
        {
            _model = model;
            _viewModel = viewModel;
            _view = view;

            _model.OnChanged += HandleModelChanged;
            _viewModel.OnProgressChanged += HandleProgressChanged;
            _viewModel.OnStatusChanged += HandleStatusChanged;
            _viewModel.OnVideoRequested += HandleVideoRequested;
            _viewModel.OnVideoStopped += HandleVideoStopped;

            SyncFromModel();
        }

        // 매 프레임 MonoBehaviour Update에서 호출
        public void Tick(float deltaTime) => _viewModel.Tick(deltaTime);

        private void SyncFromModel()
        {
            _viewModel.SetTargetProgress(_model.Progress);
            _viewModel.SetStatus(_model.StatusText);
        }

        private void HandleModelChanged() => SyncFromModel();

        private void HandleProgressChanged()
        {
            float p = _viewModel.DisplayProgress;
            _view.SetProgress(p);
            _view.SetProgressText($"{Mathf.RoundToInt(p * 100f)}%");
        }

        private void HandleStatusChanged() => _view.SetProgressText(_viewModel.StatusText);

        private void HandleVideoRequested() => _view.SetVideoPlayer(null); // caller가 IVideoPlayer 주입

        private void HandleVideoStopped() { }

        protected override void DisposeManagedResources()
        {
            _model.OnChanged -= HandleModelChanged;
            _viewModel.Dispose();
        }
    }
}
