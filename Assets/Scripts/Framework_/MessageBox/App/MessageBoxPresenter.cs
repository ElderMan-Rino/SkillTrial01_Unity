/*
using Cysharp.Threading.Tasks;
using Elder.Framework.MessageBox.Interfaces;
using Elder.Framework.UI.App;

namespace Elder.Framework.MessageBox.App
{
    // [TODO] Pool 재사용 전제: Coordinator의 ObjectPool<IMessageBoxPresenter>에서 Get/Release로 순환.
    // Release 시 Dispose가 아닌 SetViewModel(null) 등 상태 초기화만 수행 — 인스턴스 자체는 살아있어야 재사용 가능.
    internal sealed class MessageBoxPresenter : UIPresenterBase, IMessageBoxPresenter
    {
        private IMessageBoxViewModel _viewModel;

        protected override void OnInjectDependency()
        {
        }

        public void SetViewModel(IMessageBoxViewModel viewModel) => _viewModel = viewModel;

        public UniTask OnViewShownAsync() => UniTask.CompletedTask;

        public UniTask OnViewHiddenAsync() => UniTask.CompletedTask;

        public UniTask OnViewReleasedAsync() => UniTask.CompletedTask;

        protected override void DisposeManagedResources()
        {
            _viewModel = null;
        }
    }
}

*/
