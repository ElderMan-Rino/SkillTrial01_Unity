using Elder.Framework.Boot.Messages;
using Elder.Framework.Core;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.Scene.Messages;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Boot.App
{
    internal sealed class GameStartService : BaseSystem
    {
        private const string InitializeSceneKey = "Initialize";

        private ISignalRouter _router;

        private SignalToken _subscription;

        protected override bool OnInjectDependency()
        {
            TryGetSystem<ISignalRouter>(out _router);
            return true;
        }

        public override bool TryInitialize()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (부트 시점)
            _subscription = _router.Subscribe<SystemReadySignal>(HandleSystemReady);
            return true;
        }

        private void HandleSystemReady(in SystemReadySignal msg)
        {
            _router.Publish(new SceneTransitionSignal(InitializeSceneKey, SceneTransitionMode.Covered));
        }

        protected override void OnDispose()
        {
            _subscription.Dispose();
        }
    }
}
