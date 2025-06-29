using System.Collections.Generic;
using Elder.Framework.Common.Interfaces;
using Elder.Framework.Common.Messages;
using Elder.Framework.Core;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Common.App
{
    internal sealed class ScopeDisposeSystem : BaseSystem, IScopeDisposeSystem
    {
        private ISignalRouter _router;
        // [HEAP] List 초기화 시 1회 할당
        private readonly List<IScopedSystem> _systems = new();

        private SignalToken _subscription;

        protected override bool OnInjectDependency()
        {
            TryGetSystem<ISignalRouter>(out _router);
            return true;
        }

        public override bool TryInitialize()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (초기화 시점)
            _subscription = _router.Subscribe<SystemDisposeSignal>(HandleSystemDispose);
            return true;
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

        protected override void OnDispose()
        {
            _subscription.Dispose();
        }
    }
}
