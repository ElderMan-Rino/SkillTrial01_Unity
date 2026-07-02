using Elder.Framework.Core;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.GameMode.App
{
    internal sealed class GameModeOrchestratorFactory : BaseSystem, IGameModeOrchestratorFactory
    {
        private readonly Dictionary<string, Func<IGameSystemRegistry, IGameModeOrchestrator>> _funcs = new();

        protected override void HandleInjectDependency() {}

        public bool HaveOrchestrator(string sceneKey) => _funcs.ContainsKey(sceneKey);

        public bool TryAddOrchestratorCreator(string sceneKey, Func<IGameSystemRegistry, IGameModeOrchestrator> creator)
        {
            return _funcs.TryAdd(sceneKey, creator);
        }

        public bool TryCreateOrchestrator(string sceneKey, IGameSystemRegistry registry, out IGameModeOrchestrator orchestrator)
        {
            orchestrator = null;
            if (!_funcs.TryGetValue(sceneKey, out var creator)) return false;
            orchestrator = creator.Invoke(registry);
            return true;
        }
    }
}
