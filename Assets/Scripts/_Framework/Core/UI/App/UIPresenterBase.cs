using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.Interfaces;

namespace Elder.Framework.Core.UI.App
{
    public abstract class UIPresenterBase : IUIPresenter
    {
        protected IComponentProvider _provider { get; private set; }
        protected IUIViewModel _viewModel { get; private set; }

        public abstract void BindView(IUIView view);

        public void InjectDependency(IComponentProvider provider)
        {
            _provider = provider;
            OnInjectDependency();
        }

        protected virtual void OnInjectDependency() { }

        // Sleep 동안 Presenter는 Dispose되지 않으므로 VM도 그대로 유지된다.
        // Wake 시 BindView가 재호출되어도 _viewModel은 이미 존재하면 재생성하지 않고 그대로 재사용한다.
        protected TViewModel GetOrCreateViewModel<TViewModel>(System.Func<TViewModel> factory) where TViewModel : IUIViewModel
        {
            if (_viewModel is TViewModel existing)
                return existing;

            var created = factory();
            _viewModel = created;
            return created;
        }

        public void Dispose()
        {
            OnDispose();
            _viewModel?.Dispose();
            _viewModel = null;
            _provider = null;
        }

        protected virtual void OnDispose()
        {
        }
    }
}
