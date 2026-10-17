/*
using Elder.Framework.Core.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Core
{
    // [설계] FixedUpdate 단계에 필요한 모든 시스템(IFixedTickable)의 단일 진입점.
    // FrameworkRoot.FixedUpdate()는 이 FixedTick()만 호출 → 물리 동기화 프레임 순회.
    internal sealed class FrameFixedTickDispatcher : BaseSystem, IFrameFixedTickDispatcher
    {
        private List<IFixedTickable> _fixedTickables;

        protected override void HandleInjectDependency()
        {
            _fixedTickables = new List<IFixedTickable>(); // [HEAP] 1회 할당
            TryGetSystems<IFixedTickable>(ref _fixedTickables);
        }

        public void FixedTick()
        {
            for (int i = 0; i < _fixedTickables.Count; i++) _fixedTickables[i].FixedTick();
        }

        protected override void DisposeManagedResources() { }
    }
}

*/
