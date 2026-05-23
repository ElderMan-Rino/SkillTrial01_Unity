using System.Collections.Generic;
using Elder.Framework.Common.Base;
using Elder.Framework.Common.Interfaces;
using Elder.Framework.Common.Messages;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Common.App
{
    public sealed class ScopeDisposeSystem : DisposableBase, IScopeDisposeSystem
    {
        private readonly ISignalRouter _router;
        // [HEAP] List 초기화 시 1회 할당
        private readonly List<IScopedSystem> _systems = new();

        private SignalToken _subscription;

        public ScopeDisposeSystem(ISignalRouter router)
        {
            _router = router;
        }

        public void Initialize()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (초기화 시점)
            _subscription = _router.Subscribe<SystemDisposeSignal>(HandleSystemDispose);
        }

        public void Register(IScopedSystem system)
        {
            _systems.Add(system);
        }

        private void HandleSystemDispose(in SystemDisposeSignal signal)
        {
            for (int i = 0; i < _systems.Count; i++)
                _systems[i].OnRelease();

            _systems.Clear();
        }

        protected override void DisposeManagedResources()
        {
            _subscription.Dispose();
            base.DisposeManagedResources();
        }
    }
}
