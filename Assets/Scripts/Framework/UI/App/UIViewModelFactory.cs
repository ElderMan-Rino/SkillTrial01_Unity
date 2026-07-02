using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.UI.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.UI.App
{
    internal sealed class UIViewModelFactory : BaseSystem, IUIViewModelFactory, IUIViewModelRegistry
    {
        // [HEAP] 타입당 factory 델리게이트 1회 등록 — 초기화 시점에만 발생
        private readonly Dictionary<Type, Func<object>> _factories = new();
        // [HEAP] 타입당 1회 할당 — 생성 후 재사용
        private readonly Dictionary<Type, object> _instances = new();

        protected override void HandleInjectDependency() { }

        public void Register<TViewModel>(Func<TViewModel> factory) where TViewModel : class
        {
            // [HEAP] Func<TViewModel> → Func<object> 래핑 — Register 시 1회
            _factories[typeof(TViewModel)] = () => factory();
        }

        public TViewModel Create<TViewModel>() where TViewModel : class
        {
            var type = typeof(TViewModel);
            if (_instances.TryGetValue(type, out var existing)) return (TViewModel)existing;
            if (!_factories.TryGetValue(type, out var factory))
                throw new InvalidOperationException($"ViewModel '{type.Name}' is not registered.");
            // [HEAP] ViewModel 인스턴스 — 씬 진입 시 타입당 1회
            var instance = factory();
            _instances[type] = instance;
            return (TViewModel)instance;
        }

        public void Release<TViewModel>() where TViewModel : class
        {
            if (_instances.Remove(typeof(TViewModel), out var instance) && instance is IDisposable disposable)
                disposable.Dispose();
        }

        public override UniTask InitializeAsync() => UniTask.CompletedTask;
        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;

        protected override void DisposeManagedResources()
        {
            foreach (var instance in _instances.Values)
            {
                if (instance is IDisposable disposable) disposable.Dispose();
            }
            _instances.Clear();
            _factories.Clear();
        }
    }
}
