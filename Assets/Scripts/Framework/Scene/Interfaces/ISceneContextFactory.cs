using Elder.Framework.Scene.Domain.Data;

namespace Elder.Framework.Scene.Interfaces
{
    public interface ISceneContextFactory
    {
        public SceneLoadContext Create(string mainSceneName);
        public void Release(SceneLoadContext context);
    }
}