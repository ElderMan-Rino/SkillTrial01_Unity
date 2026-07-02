using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.Interfaces;
using System;

namespace Elder.Game.SkillTrial.GameMode.App
{
    public sealed class SkillTrialGameModeInitializer : BaseSystem, IGameSystem
    {
        private IGameModeOrchestratorFactory _factory;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IGameModeOrchestratorFactory>(out _factory))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IGameModeOrchestratorFactory)}");
        }

        public override UniTask InitializeAsync()
        {
            _factory.TryAddOrchestratorCreator(SkillTrialOrchestrator.SceneKey, CreateOrchestrator);
            return UniTask.CompletedTask;
        }

        private static IGameModeOrchestrator CreateOrchestrator(IGameSystemRegistry registry)
        {
            return new SkillTrialOrchestrator(registry);
        }
    }
}