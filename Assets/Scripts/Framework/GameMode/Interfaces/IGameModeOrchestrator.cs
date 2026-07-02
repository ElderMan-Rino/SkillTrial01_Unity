using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.GameMode.Interfaces
{
    public interface IGameModeOrchestrator : ISystemComponent
    {
        public Type GameModeType { get; }
        public UniTask TeardownAsync();
        public UniTask<bool> TryPrepareAsync();
        public UniTask<bool> TryActivateAsync();
    }
}