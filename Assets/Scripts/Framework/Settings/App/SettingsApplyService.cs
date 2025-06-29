using System.Collections.Generic;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Core;
using Elder.Framework.Settings.Interfaces;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Settings.App
{
    internal sealed class SettingsApplyService : BaseSystem
    {
        private ISignalRouter _router;
        private IAppSettingsStore _store;
        // [HEAP] 빌드 시점 1회 할당
        private readonly List<ISettingsApplicable> _applicables = new();

        private SignalToken _subscription;

        protected override bool OnInjectDependency()
        {
            TryGetSystem<ISignalRouter>(out _router);
            TryGetSystem<IAppSettingsStore>(out _store);
            return true;
        }

        public override bool TryInitialize()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (초기화 시점)
            _subscription = _router.Subscribe<PreSystemReadySignal>(HandlePreSystemReady);
            return true;
        }

        public void RegisterApplicable(ISettingsApplicable applicable)
        {
            _applicables.Add(applicable);
        }

        private void HandlePreSystemReady(in PreSystemReadySignal signal)
        {
            _store.Load();

            for (int i = 0; i < _applicables.Count; i++)
                _applicables[i].ApplySettings(_store);

            _router.Publish(new SystemReadySignal());
        }

        protected override void OnDispose()
        {
            _subscription.Dispose();
        }
    }
}
