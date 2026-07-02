using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.GameMode.Interfaces
{
    public interface IFlowModule : IGameSystem
    {
        public UniTask ExecuteAsync();
    }
}
