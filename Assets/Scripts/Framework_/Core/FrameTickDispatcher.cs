/*
using Elder.Framework.Core.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Core
{
    // [설계] 매프레임 Tick이 필요한 모든 시스템(ITickable)의 단일 진입점.
    // FrameworkRoot.Update()는 이 Tick()만 호출 → 등록된 ITickable들을 순회 호출.
    internal sealed class FrameTickDispatcher : BaseSystem, IFrameTickDispatcher
    {
        private List<ITickable> _tickables;

        protected override void HandleInjectDependency()
        {
            _tickables = new List<ITickable>(); // [HEAP] 1회 할당
            TryGetSystems<ITickable>(ref _tickables);
        }

        public void Tick()
        {
            for (int i = 0; i < _tickables.Count; i++) _tickables[i].Tick();
        }

        protected override void DisposeManagedResources() { }
    }
}

*/
