using Elder.Framework.Scene.Domain.Data;
using Elder.Framework.Scene.Interfaces;
using UnityEngine.Pool;

namespace Elder.Framework.Scene.Infra
{
    internal sealed class SceneContextFactory : ISceneContextFactory
    {
        private readonly IObjectPool<SceneLoadContext> _pool;

        public SceneContextFactory()
        {
            _pool = new ObjectPool<SceneLoadContext>(createFunc: CreateFunc, actionOnRelease: OnContextReleasesd);
        }

        private SceneLoadContext CreateFunc()
        {
            return new SceneLoadContext();
        }

        private void OnContextReleasesd(SceneLoadContext context)
        {
            context.Dispose();
        }

        public SceneLoadContext Create(string mainSceneName)
        {
            return _pool.Get().Initialize(mainSceneName);
        }

        public void Release(SceneLoadContext context)
        {
            _pool.Release(context);
        }
    }
}