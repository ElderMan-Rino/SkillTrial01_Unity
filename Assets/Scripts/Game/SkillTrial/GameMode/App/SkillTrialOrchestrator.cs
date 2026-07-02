using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.App;
using System;

namespace Elder.Game.SkillTrial.GameMode.App
{
    public class SkillTrialOrchestrator : GameModeOrchestrator
    {
        public const string SceneKey = "SkillTrial";

        public override Type GameModeType => typeof(SkillTrialOrchestrator);

        public SkillTrialOrchestrator(IGameSystemRegistry registry) : base(registry) { }

        protected override void RegisterSystems(IGameSystemRegistry registry)
        {
        }

        protected override async UniTask<bool> OnPrepareAsync()
        {
            await UniTask.CompletedTask;
            return true;
        }

        public override async UniTask<bool> TryActivateAsync()
        {
            await UniTask.CompletedTask;
            return true;
        }

        public override async UniTask TeardownAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}
