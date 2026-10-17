/*
using Elder.Framework.Core.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Core
{
    // [설계] LateUpdate 단계에 필요한 모든 시스템(ILateTickable)의 단일 진입점.
    // FrameworkRoot.LateUpdate()는 이 LateTick()만 호출 → Tick() 결과를 소비하는 후처리 순회.
    internal sealed class FrameLateTickDispatcher : BaseSystem, IFrameLateTickDispatcher
    {
        private List<ILateTickable> _lateTickables;

        protected override void HandleInjectDependency()
        {
            _lateTickables = new List<ILateTickable>(); // [HEAP] 1회 할당
            TryGetSystems<ILateTickable>(ref _lateTickables);
        }

        public void LateTick()
        {
            for (int i = 0; i < _lateTickables.Count; i++) _lateTickables[i].LateTick();
        }

        protected override void DisposeManagedResources() { }
    }
}

*/
