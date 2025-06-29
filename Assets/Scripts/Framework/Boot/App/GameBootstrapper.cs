using Elder.Framework.Boot.Definitions.Constants;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Common.Base;
using Elder.Framework.Flux.Interfaces;
using Elder.Framework.Scene.Messages;
using VContainer.Unity;

namespace Elder.Framework.Boot.App
{
    public class GameBootStrapper : DisposableBase, IStartable
    {
        private readonly IFluxRouter _router;
        private readonly IStartupEnvironment _startUp;

        public GameBootStrapper(IFluxRouter router, IStartupEnvironment startUp)
        {
            _router = router;
            _startUp = startUp;
        }

        public void Start()
        {
            if (!IsInitSceneActive())
                return;

            PublishBootSequence();
        }

        private bool IsInitSceneActive()
        {
            return _startUp.IsInitSceneActive();
        }

        private void PublishBootSequence()
        {
            _router?.Publish<FxInitializeSystem>(new FxInitializeSystem());
            _router?.Publish<FxSceneTransition>(new FxSceneTransition(BootConstants.BootStrapSceneKey));
        }
    }
}