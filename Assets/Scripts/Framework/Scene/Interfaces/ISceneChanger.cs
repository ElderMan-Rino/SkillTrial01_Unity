using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Scene.Interfaces
{
    public interface ISceneChanger : ISystemComponent
    {
        public UniTask<bool> ExecuteAsync(string targetSceneKey);
    }
}
