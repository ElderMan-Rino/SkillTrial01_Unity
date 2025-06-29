using Cysharp.Threading.Tasks;

namespace Elder.Framework.Scene.Interfaces
{
    internal interface ISceneTransitionExecutor
    {
        public UniTask<bool> ExecuteAsync(string targetSceneKey);
    }
}
