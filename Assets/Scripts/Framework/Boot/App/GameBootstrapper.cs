using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Common.Messages;
using Elder.Framework.Common.Base;
using Elder.Framework.Data.Messages;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Flux.Interfaces;
using VContainer.Unity;

namespace Elder.Framework.Boot.App
{
    public sealed class GameBootStrapper : DisposableBase, IStartable
    {
        private readonly IFluxRouter _router;
        private readonly IStartupEnvironment _startUp;

        private SubscriptionToken _dataInitializedSubscription;

        public GameBootStrapper(IFluxRouter router, IStartupEnvironment startUp)
        {
            _router = router;
            _startUp = startUp;
        }

        public void Start()
        {
            if (!_startUp.IsInitSceneActive())
                return;

            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (부트 시점)
            _dataInitializedSubscription = _router.Subscribe<FxBaseDataInitialized>(HandleBaseDataInitialized);
            _router.Publish(new FxInitializeSystem());
            _router.Publish(new FxPostInitializeSystem());
        }

        private void HandleBaseDataInitialized(in FxBaseDataInitialized msg)
        {
            _dataInitializedSubscription.Dispose();
            _router.Publish(new FxSystemInitializeEnd());
        }

        protected override void DisposeManagedResources()
        {
            _dataInitializedSubscription.Dispose();
            base.DisposeManagedResources();
        }
    }
}
