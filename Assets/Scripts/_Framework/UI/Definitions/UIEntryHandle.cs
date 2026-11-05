using Cysharp.Threading.Tasks;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;

namespace Elder.Framework.UI.Definitions
{
    // Coordinator가 OpenAsync 호출 전 직접 생성하는 등록 단위.
    internal sealed class UIEntryHandle : IUIEntryHandle
    {
        private readonly int _scope;
        private readonly int _displayIndex;
        private IUIViewHandle _viewHandle;
        private readonly UIPresenterBase _presenter;
        private readonly IUIViewModel _viewModel;
        private readonly IUIRebindStrategy _rebindStrategy;

        public int Scope => _scope;

        public int DisplayIndex => _displayIndex;

        public IUIViewHandle ViewHandle => _viewHandle;

        public UIPresenterBase Presenter => _presenter;

        public IUIViewModel ViewModel => _viewModel;

        public UIEntryHandle(int scope, int displayIndex, IUIViewHandle viewHandle, UIPresenterBase presenter, IUIViewModel viewModel, IUIRebindStrategy rebindStrategy)
        {
            _scope = scope;
            _displayIndex = displayIndex;
            _viewHandle = viewHandle;
            _presenter = presenter;
            _viewModel = viewModel;
            _rebindStrategy = rebindStrategy;
        }

        public void Sleep()
        {
            _viewHandle.View.UnbindRoot();
            _viewHandle.DestroyViewOnly();
        }

        // 모든 View 타입(Addressable Destroy/Recreate, Scene SetActive)이 동일하게 재바인딩 전략을 재호출해 UI 트리를 복원한다.
        public void Wake()
        {
            _viewHandle.Recreate();
            _rebindStrategy?.Rebind(_viewHandle.View);
            _presenter.BindView(_viewHandle.View);
        }

        public UniTask<bool> RequestCloseAsync()
        {
            return _presenter.RequestCloseAsync();
        }
    }
}
