using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.Interfaces;
using Elder.Framework.Scene.Definitions;
using System;

namespace Elder.Framework.GameMode.Preload.App
{
    internal sealed class PreloadOrchestratorRegistrar : BaseSystem, IGameSystem
    {
        private IGameModeOrchestratorFactory _factory;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IGameModeOrchestratorFactory>(out _factory))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IGameModeOrchestratorFactory)}");
        }

        public override UniTask InitializeAsync()
        {
            //_factory.TryAddOrchestratorCreator(SceneConstants.PreloadSceneKey, registry => new PreloadOrchestrator(registry));
            return UniTask.CompletedTask;
        }
    }
}
