using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.GameMode.App
{
    // 용도: 실제 게임 플레이 씬. IGameModeRouter + FlowDriver + PhaseStateMachine + Ruleset 조립
    // 구현: CreateModeRouter() / CreateRule() / CreateSession() / CreateStates() 선언만으로 완성
    // TODO: IGameContext / IGameModeRouter / IGameFlowDriver / IGameRuleset / IGameEventBridge /
    //       GamePhaseStateMachine / IGamePhaseState 인터페이스 구현 후 연결
    public abstract class GameOrchestrator : GameModeOrchestrator
    {
        protected GameOrchestrator(IGameSystemRegistry registry) : base(registry) { }

        public override Type GameModeType => GetType();

        protected override void RegisterSystems(IGameSystemRegistry registry) { }

        protected override UniTask<bool> OnPrepareAsync() => UniTask.FromResult(true);

        public override UniTask<bool> TryActivateAsync() => UniTask.FromResult(true);

        public override UniTask TeardownAsync() => UniTask.CompletedTask;
    }
}
