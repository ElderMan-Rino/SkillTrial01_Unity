/*
using Elder.Framework.Core.Interfaces;
using UnityEngine;

namespace Elder.Framework.MainFrame.Infra
{
    // [설계] Unity 프레임 주기(FixedUpdate/Update/LateUpdate)를 각 단계별 디스패처로 릴레이하는 전용 컴포넌트.
    // FrameworkRoot의 부트 오케스트레이션 책임과 분리 — SRP.
    internal sealed class FrameworkTickRelay : MonoBehaviour
    {
        private IGameSystemProvider _provider;

        public void Initialize(IGameSystemProvider provider)
        {
            _provider = provider;
        }

        private void FixedUpdate()
        {
            if (_provider is not null && _provider.TryGetSystem<IFrameFixedTickDispatcher>(out var dispatcher)) dispatcher.FixedTick();
        }

        private void Update()
        {
            if (_provider is not null && _provider.TryGetSystem<IFrameTickDispatcher>(out var dispatcher)) dispatcher.Tick();
        }

        private void LateUpdate()
        {
            if (_provider is not null && _provider.TryGetSystem<IFrameLateTickDispatcher>(out var dispatcher)) dispatcher.LateTick();
        }
    }
}
*/
