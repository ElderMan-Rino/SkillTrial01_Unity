using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Core;
using Elder.Framework.Settings.Interfaces;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using System;

namespace Elder.Framework.Settings.App
{
    internal sealed class SettingsApplyService : BaseSystem
    {
        private ISignalRouter _router;
        private IAppSettingsStore _store;
        // [HEAP] 빌드 시점 1회 할당
        private readonly List<ISettingsApplicable> _applicables = new();

        private SignalToken _subscription;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<ISignalRouter>(out _router))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
            if (!TryGetSystem<IAppSettingsStore>(out _store))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IAppSettingsStore)}");
        }

        public override UniTask InitializeAsync()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (초기화 시점)
            _subscription = _router.Subscribe<PreSystemReadySignal>(HandlePreSystemReady);
            return UniTask.CompletedTask;
        }

        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;

        public void RegisterApplicable(ISettingsApplicable applicable)
        {
            _applicables.Add(applicable);
        }

        private void HandlePreSystemReady(in PreSystemReadySignal signal)
        {
            for (int i = 0; i < _applicables.Count; i++)
                _applicables[i].ApplySettings(_store);

            _router.Publish(new SystemReadySignal());
        }

        protected override void DisposeManagedResources()
        {
            _subscription.Dispose();
        }
    }
}
