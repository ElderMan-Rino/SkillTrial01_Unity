using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Common.Messages;
using Elder.Framework.Core;
using Elder.Framework.Data.Messages;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Boot.App
{
    internal sealed class GameBootStrapper : BaseSystem
    {
        private ISignalRouter _router;
        private IStartupEnvironment _startUp;

        private SignalToken _dataInitializedSubscription;

        protected override bool OnInjectDependency()
        {
            TryGetSystem<ISignalRouter>(out _router);
            TryGetSystem<IStartupEnvironment>(out _startUp);
            return true;
        }

        public void Start()
        {
            if (!_startUp.IsInitSceneActive())
                return;

            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (부트 시점)
            _dataInitializedSubscription = _router.Subscribe<BaseDataInitializedSignal>(HandleBaseDataInitialized);
            _router.Publish(new SystemInitializeSignal());
            _router.Publish(new SystemPostInitializeSignal());
        }

        private void HandleBaseDataInitialized(in BaseDataInitializedSignal msg)
        {
            _dataInitializedSubscription.Dispose();
            _router.Publish(new PreSystemReadySignal());
        }

        protected override void OnDispose()
        {
            _dataInitializedSubscription.Dispose();
        }
    }
}
