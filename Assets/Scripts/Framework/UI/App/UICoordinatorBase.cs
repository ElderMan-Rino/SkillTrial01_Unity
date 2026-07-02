using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.UI.Interfaces;
using System;

namespace Elder.Framework.UI.App
{
    public abstract class UICoordinatorBase : BaseSystem, IUICoordinator
    {
        protected IUISystem _uiSystem;
        protected ISignalRouter _router;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IUISystem>(out _uiSystem))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IUISystem)}");
            if (!TryGetSystem<ISignalRouter>(out _router))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
            OnInjectDependency();
        }

        protected virtual void OnInjectDependency() { }

        public abstract UniTask PrepareAsync();
        public abstract UniTask ShowAsync();
        public abstract UniTask HideAsync();
    }
}
