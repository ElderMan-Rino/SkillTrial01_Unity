using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.App;
using System;

namespace Elder.Framework.GameMode.Main
{
    // TODO: GameFlow 인터페이스 구현 후 IGameModeRouter / FlowDriver / PhaseStateMachine 연결
    internal sealed class MainOrchestrator : GameOrchestrator
    {
        public MainOrchestrator(IGameSystemRegistry registry) : base(registry) { }

        public override Type GameModeType => typeof(MainOrchestrator);

        protected override void RegisterSystems(IGameSystemRegistry registry) { }

        protected override UniTask<bool> OnPrepareAsync() => UniTask.FromResult(true);

        public override UniTask<bool> TryActivateAsync() => UniTask.FromResult(true);

        public override UniTask TeardownAsync() => UniTask.CompletedTask;
    }
}
