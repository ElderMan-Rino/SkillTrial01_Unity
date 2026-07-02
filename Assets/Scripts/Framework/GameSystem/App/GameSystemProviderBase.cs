using Cysharp.Threading.Tasks;
using Elder.Framework.Common.Base;
using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.GameSystem.App
{
    internal abstract class GameSystemProviderBase : DisposableBase, IGameSystemProvider
    {
        protected readonly Dictionary<Type, ISystemComponent> _systemComponents;
        protected readonly List<ISystemComponent> _orderedSystems;

        public abstract bool TryGetSystem<T>(out T system) where T : class, ISystemComponent;
        public abstract bool TryGetSystems<T>(ref List<T> results) where T : class, ISystemComponent;

        protected GameSystemProviderBase(Dictionary<Type, ISystemComponent> systemComponents, List<ISystemComponent> orderedSystems)
        {
            _systemComponents = systemComponents;
            _orderedSystems = orderedSystems;
        }

        public bool TryDisposeSystem(Type systemType)
        {
            if (!_systemComponents.Remove(systemType, out var systemComponent)) return false;

            _orderedSystems.Remove(systemComponent);

            if (systemComponent is IDisposableBase disposable)
            {
                disposable.PreDispose();
                disposable.Dispose();
            }
            return true;
        }

        public void InjectAll()
        {
            for (int i = 0; i < _orderedSystems.Count; i++)
            {
                if (_orderedSystems[i] is not IGameSystem gameSystem) continue;
                try
                {
                    gameSystem.InjectDependency(this);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"[DI] InjectDependency failed for {_orderedSystems[i].GetType().Name}: {ex.Message}", ex);
                }
            }
        }

        public async UniTask InitializeAllAsync()
        {
            // [HEAP] UniTask[] + WhenAll — 초기화 경로, 허용
            var tasks = new UniTask[_orderedSystems.Count];
            int index = 0;
            for (int i = 0; i < _orderedSystems.Count; i++)
            {
                if (_orderedSystems[i] is not IGameSystem gameSystem) continue;
                tasks[index++] = gameSystem.InitializeAsync();
            }
            await UniTask.WhenAll(tasks);
        }

        public async UniTask PostInitializeAllAsync()
        {
            // [HEAP] UniTask[] + WhenAll — 초기화 경로, 허용
            var tasks = new UniTask[_orderedSystems.Count];
            int index = 0;
            for (int i = 0; i < _orderedSystems.Count; i++)
            {
                if (_orderedSystems[i] is not IGameSystem gameSystem) continue;
                tasks[index++] = gameSystem.PostInitializeAsync();
            }
            await UniTask.WhenAll(tasks);
        }

        public override void PreDispose()
        {
            for (int i = 0; i < _orderedSystems.Count; i++)
            {
                if (_orderedSystems[i] is not IDisposableBase disposable) continue;
                disposable.PreDispose();
            }
        }

        protected override void DisposeManagedResources()
        {
            for (int i = 0; i < _orderedSystems.Count; i++)
            {
                if (_orderedSystems[i] is not IDisposableBase disposable) continue;
                disposable.Dispose();
            }
        }
    }
}
