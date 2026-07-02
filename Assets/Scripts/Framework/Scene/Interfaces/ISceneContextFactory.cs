using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Scene.Interfaces
{
    public interface ISceneContextFactory : ISystemComponent
    {
        public ISceneLoadContext Create(string mainSceneName);
        public void Release(ISceneLoadContext context);
    }
}