using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.UI.Interfaces;
using System;

namespace Elder.Framework.UI.App
{
    public sealed class UIViewFactory : BaseSystem, IUIViewFactory
    {
        private IUISystem _uiSystem;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IUISystem>(out _uiSystem))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IUISystem)}");
        }

        public UniTask<TView> CreateAsync<TView>(string addressableKey) where TView : UIViewBase
            => _uiSystem.ShowAsync<TView>(addressableKey);

        public UniTask<TView> CreateAsync<TView>() where TView : UIViewBase
        {
            if (_uiSystem.TryGetView<TView>(out var view)) return UniTask.FromResult(view);
            throw new InvalidOperationException($"[UIViewFactory] Scene-bound view not found: {typeof(TView).Name}");
        }

        public UniTask ReleaseAsync<TView>() where TView : UIViewBase
            => _uiSystem.ReleaseAsync<TView>();

        // IUIViewFactory — Release는 동기 시그니처이므로 async Release 별도 제공
        public void Release<TView>() where TView : UIViewBase
            => _uiSystem.ReleaseAsync<TView>().Forget();

        public override UniTask InitializeAsync() => UniTask.CompletedTask;
        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;
    }
}
