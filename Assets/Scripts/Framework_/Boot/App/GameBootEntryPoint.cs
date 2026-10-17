/*
using Elder.Framework.Boot.Definitions;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Core;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.Scene.Messages;
using Elder.Framework.Signal.Interfaces;
using System;

namespace Elder.Framework.Boot.App
{
    internal sealed class GameBootEntryPoint : BaseSystem, IGameBootEntryPoint
    {
        private ISignalRouter _router;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<ISignalRouter>(out _router))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
        }

        public void Run(SceneBootMode bootMode)
        {
            if (bootMode == SceneBootMode.SystemsOnly) return;
            _router.Publish(new SceneTransitionSignal(SceneConstants.SplashSceneKey));
        }
    }
}

*/
