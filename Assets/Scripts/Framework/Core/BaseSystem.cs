using Elder.Framework.Common.Base;
using Elder.Framework.Core.Interfaces;
using R3;

namespace Elder.Framework.Core
{
    public abstract class BaseSystem : DisposableBase, IGameSystem
    {
        private IGameSystemProvider _provider;

        // [HEAP] 초기화 시 1회 할당 — R3 구독 토큰 일괄 해제용
        protected readonly CompositeDisposable _disposables = new();

        public bool TryInjectDependency(IGameSystemProvider provider)
        {
            _provider = provider;
            return OnInjectDependency();
        }

        public virtual bool TryInitialize() => true;

        public virtual bool TryPostInitialize() => true;

        public void TryDispose() => Dispose();

        protected bool TryGetSystem<T>(out T system) where T : class, ISystemComponent => _provider.TryGetSystem<T>(out system);

        /// <summary>[Dispose 단계 1] 구독 해제 등 중단 작업.</summary>
        protected override void OnDisposing()
        {
            _disposables.Dispose();
            OnPreDispose();
        }

        /// <summary>[Dispose 단계 2] 관리 리소스 해제.</summary>
        protected override void DisposeManagedResources() => OnDispose();

        /// <summary>[Dispose 단계 4] 해제 완료 후 후처리.</summary>
        protected override void FinalizeDispose() => OnPostDispose();
        protected virtual void OnPreDispose() { }
        protected virtual void OnDispose() { }
        protected virtual void OnPostDispose() { }
        protected virtual bool OnInjectDependency() => true;
    }
}
