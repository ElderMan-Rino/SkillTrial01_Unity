using Elder.Framework.Boot.Definitions.Constants;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Common.Base;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Scene.Messages;

namespace Elder.Framework.Boot.App
{
    public sealed class GameStartService : DisposableBase
    {
        private readonly ISignalRouter _router;

        private SignalToken _subscription;

        public GameStartService(ISignalRouter router)
        {
            _router = router;
        }

        public void Initialize()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (부트 시점)
            _subscription = _router.Subscribe<SystemInitializeEndSignal>(HandleSystemInitializeEnd);
        }

        private void HandleSystemInitializeEnd(in SystemInitializeEndSignal msg)
        {
            _subscription.Dispose();
            _router.Publish(new SceneTransitionSignal(BootConstants.BootStrapSceneKey));
        }

        protected override void DisposeManagedResources()
        {
            _subscription.Dispose();
            base.DisposeManagedResources();
        }
    }
}
