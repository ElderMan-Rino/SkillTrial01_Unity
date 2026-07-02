using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.App;
using Elder.Framework.UI.Interfaces;
using System;

namespace Elder.Framework.GameMode.Title.App
{
    internal sealed class TitleOrchestrator : GameModeOrchestrator
    {
        public TitleOrchestrator(IGameSystemRegistry registry) : base(registry) { }

        public override Type GameModeType => typeof(TitleOrchestrator);

        protected override void RegisterSystems(IGameSystemRegistry registry)
        {
            registry.Register<TitleCoordinator>().As<IUICoordinator>();
        }

        protected override UniTask<bool> OnPrepareAsync() => UniTask.FromResult(true);

        public override async UniTask<bool> TryActivateAsync()
        {
            if (!_provider.TryGetSystem<IUICoordinator>(out var coordinator)) return false;
            await coordinator.ShowAsync();
            return true;
        }

        public override async UniTask TeardownAsync()
        {
            if (_provider.TryGetSystem<IUICoordinator>(out var coordinator))
                await coordinator.HideAsync();
        }
    }
}
