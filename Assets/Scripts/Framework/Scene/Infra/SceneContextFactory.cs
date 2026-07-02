using Elder.Framework.Scene.Domain.Models;
using Elder.Framework.Scene.Interfaces;
using System;
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

        private static SceneLoadContext CreateFunc()
        {
            return new SceneLoadContext(); // [HEAP] 풀 초과 시 새 인스턴스 생성
        }

        private static void OnContextReleased(SceneLoadContext context)
        {
            context.Dispose();
        }

        public ISceneLoadContext Create(string mainSceneName)
        {
            return _pool.Get().Initialize(mainSceneName);
        }

        public void Release(ISceneLoadContext context)
        {
            if (context is not SceneLoadContext concrete)
                throw new ArgumentException($"[Pool] Unexpected context type: {context?.GetType().Name}", nameof(context));
            _pool.Release(concrete);
        }
    }
}