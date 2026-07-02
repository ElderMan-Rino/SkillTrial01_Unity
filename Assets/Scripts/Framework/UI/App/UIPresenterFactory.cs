using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.UI.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.UI.App
{
    internal sealed class UIPresenterFactory : BaseSystem, IUIPresenterFactory, IUIPresenterRegistry
    {
        // [HEAP] 인터페이스 타입 → factory 델리게이트 1회 등록
        private readonly Dictionary<Type, Func<IUIPresenter>> _factories = new();
        // [HEAP] 인터페이스 타입 → 인스턴스 1회 할당 후 재사용
        private readonly Dictionary<Type, IUIPresenter> _instances = new();
        // [HEAP] View 타입 → Presenter 인터페이스 타입 매핑 — RegisterForView 시 1회
        private readonly Dictionary<Type, Type> _viewToPresenter = new();

        protected override void HandleInjectDependency() { }

        public void Register<TInterface, TPresenter>(Func<TPresenter> factory)
            where TInterface : IUIPresenter
            where TPresenter : TInterface
        {
            // [HEAP] Func<TPresenter> → Func<IUIPresenter> 래핑 — Register 시 1회
            _factories[typeof(TInterface)] = () => factory();
        }

        public void RegisterForView<TView, TInterface>()
            where TView : UIViewBase
            where TInterface : class, IUIPresenter, IUIPresenterLifecycle
        {
            _viewToPresenter[typeof(TView)] = typeof(TInterface);
        }

        public TInterface Create<TInterface>() where TInterface : class, IUIPresenter
        {
            var type = typeof(TInterface);
            if (_instances.TryGetValue(type, out var existing)) return (TInterface)existing;
            if (!_factories.TryGetValue(type, out var factory))
                throw new InvalidOperationException($"Presenter '{type.Name}' is not registered.");
            // [HEAP] Presenter 인스턴스 — 씬 진입 시 인터페이스 타입당 1회
            var instance = factory();
            instance.InjectProvider(_systemProvider);
            _instances[type] = instance;
            return (TInterface)instance;
        }

        public void Release<TInterface>() where TInterface : class, IUIPresenter
        {
            if (_instances.Remove(typeof(TInterface), out var presenter)) presenter.Dispose();
        }

        public IUIPresenterLifecycle GetLifecycleForView<TView>() where TView : UIViewBase
        {
            if (!_viewToPresenter.TryGetValue(typeof(TView), out var presenterType)) return null;
            if (!_instances.TryGetValue(presenterType, out var presenter)) return null;
            return presenter as IUIPresenterLifecycle;
        }

        public override UniTask InitializeAsync() => UniTask.CompletedTask;
        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;

        protected override void DisposeManagedResources()
        {
            // [HEAP] Dictionary 열거자 할당
            foreach (var presenter in _instances.Values) presenter.Dispose();
            _instances.Clear();
            _factories.Clear();
            _viewToPresenter.Clear();
        }
    }
}
