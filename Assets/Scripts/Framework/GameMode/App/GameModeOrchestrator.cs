using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.Interfaces;
using System;

namespace Elder.Framework.GameMode.App
{
    public abstract class GameModeOrchestrator : BaseSystemComponent, IGameModeOrchestrator
    {
        private readonly IGameSystemRegistry _registry;
        protected IGameSystemProvider _provider;

        protected GameModeOrchestrator(IGameSystemRegistry registry)
        {
            _registry = registry;
        }

        public abstract Type GameModeType { get; }
        public abstract UniTask TeardownAsync();
        public abstract UniTask<bool> TryActivateAsync();

        public async UniTask<bool> TryPrepareAsync()
        {
            RegisterSystems(_registry);
            _provider = _registry.Build();
            _provider.InjectAll();
            await _provider.InitializeAllAsync();
            await _provider.PostInitializeAllAsync();
            return await OnPrepareAsync();
        }

        protected virtual void RegisterSystems(IGameSystemRegistry registry) { }
        
        protected virtual UniTask<bool> OnPrepareAsync() => UniTask.FromResult(true);

        public override void PreDispose()
        {
            if (_provider is IDisposableBase disposable) disposable.PreDispose();
        }

        protected override void DisposeManagedResources()
        {
            if (_provider is IDisposableBase disposable) disposable.Dispose();
        }
    }
}
