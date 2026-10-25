using Cysharp.Threading.Tasks;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;

namespace Elder.Framework.UI.Definitions
{
    // Coordinator가 OpenAsync 호출 전 직접 생성하는 등록 단위. 인터페이스/클래스 틀만 존재, 로직은 미구현.
    internal sealed class UIEntryHandle : IUIEntryHandle
    {
        private IUIViewHandle _viewHandle;
        private readonly UIPresenterBase _presenter;
        private readonly IUIViewModel _viewModel;

        public IUIViewHandle ViewHandle => _viewHandle;

        public UIPresenterBase Presenter => _presenter;

        public IUIViewModel ViewModel => _viewModel;

        public UIEntryHandle(IUIViewHandle viewHandle, UIPresenterBase presenter, IUIViewModel viewModel)
        {
            _viewHandle = viewHandle;
            _presenter = presenter;
            _viewModel = viewModel;
        }

        public void Sleep()
        {
        }

        public void Wake(IUIViewHandle viewHandle)
        {
        }

        public UniTask<bool> RequestCloseAsync()
        {
            return UniTask.FromResult(true);
        }
    }
}
