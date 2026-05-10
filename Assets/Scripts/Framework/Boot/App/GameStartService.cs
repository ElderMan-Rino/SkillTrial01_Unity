using Elder.Framework.Boot.Definitions.Constants;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Common.Base;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Flux.Interfaces;
using Elder.Framework.Scene.Messages;
using VContainer.Unity;

namespace Elder.Framework.Boot.App
{
    public sealed class GameStartService : DisposableBase, IInitializable
    {
        private readonly IFluxRouter _router;

        private SubscriptionToken _subscription;

        public GameStartService(IFluxRouter router)
        {
            _router = router;
        }

        public void Initialize()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (부트 시점)
            _subscription = _router.Subscribe<FxSystemInitializeEnd>(HandleSystemInitializeEnd);
        }

        private void HandleSystemInitializeEnd(in FxSystemInitializeEnd msg)
        {
            _subscription.Dispose();
            _router.Publish(new FxSceneTransition(BootConstants.BootStrapSceneKey));
        }

        protected override void DisposeManagedResources()
        {
            _subscription.Dispose();
            base.DisposeManagedResources();
        }
    }
}
