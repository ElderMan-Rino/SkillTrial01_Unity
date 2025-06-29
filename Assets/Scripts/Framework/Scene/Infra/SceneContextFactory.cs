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
            // [HEAP] ObjectPool 생성자 — createFunc/actionOnRelease 델리게이트 1회 할당
            _pool = new ObjectPool<SceneLoadContext>(createFunc: CreateFunc, actionOnRelease: OnContextReleased);
        }

        private SceneLoadContext CreateFunc()
        {
            // [HEAP] 풀 초과 시 새 인스턴스 생성
            return new SceneLoadContext();
        }

        private void OnContextReleased(SceneLoadContext context)
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