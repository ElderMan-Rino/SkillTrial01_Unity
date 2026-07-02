using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.App;
using System;

namespace Elder.Framework.GameMode.Preload.App
{
    internal sealed class PreloadOrchestrator : GameModeOrchestrator
    {
        public PreloadOrchestrator(IGameSystemRegistry registry) : base(registry) { }

        public override Type GameModeType => typeof(PreloadOrchestrator);

        public override UniTask<bool> TryActivateAsync() => UniTask.FromResult(true);

        public override UniTask TeardownAsync() => UniTask.CompletedTask;
    }
}
