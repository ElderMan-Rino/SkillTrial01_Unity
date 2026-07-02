using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.GameMode.Interfaces
{
    public interface IGameModeOrchestratorFactory : ISystemComponent
    {
        public bool HaveOrchestrator(string sceneKey);
        public bool TryCreateOrchestrator(string sceneKey, IGameSystemRegistry registry, out IGameModeOrchestrator orchestrator);
        public bool TryAddOrchestratorCreator(string sceneKey, Func<IGameSystemRegistry, IGameModeOrchestrator> creator);
    }
}
