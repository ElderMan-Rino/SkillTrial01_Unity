using Cysharp.Threading.Tasks;
using Elder.Framework.Common.Base;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.UI.App
{
    public abstract class UIPresenterBase : DisposableBase, IUIPresenter
    {
        protected IGameSystemProvider _systemProvider { get; private set; }

        public void InjectProvider(IGameSystemProvider provider)
        {
            _systemProvider = provider;
            OnInjectDependency();
        }

        protected virtual void OnInjectDependency() { }

        protected bool TryGetSystem<T>(out T system) where T : class, ISystemComponent
            => _systemProvider.TryGetSystem<T>(out system);

        protected bool TryGetSystems<T>(ref List<T> results) where T : class, ISystemComponent
            => _systemProvider.TryGetSystems<T>(ref results);

        public virtual UniTask OnShowAsync() => UniTask.CompletedTask;
        public virtual UniTask OnHideAsync() => UniTask.CompletedTask;

        public override void PreDispose() { }
    }
}
