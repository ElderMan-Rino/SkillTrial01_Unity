using Elder.Framework.Core;
using Elder.Framework.Input.Infra.Generated;
using Elder.Framework.Input.Interfaces;
using UnityEngine.InputSystem;

namespace Elder.Framework.Input.Infra
{
    internal sealed class InputControlCenter : BaseSystem, IInputControlCenter
    {
        private readonly GameInputActions _actions;

        public InputControlCenter()
        {
            // [HEAP] GameInputActions 생성 — 등록 시점 1회
            _actions = new GameInputActions();
        }

        public void Enable() => _actions.Enable();
        public void Disable() => _actions.Disable();
        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
            => _actions.FindAction(actionNameOrId, throwIfNotFound);

        protected override void DisposeManagedResources()
        {
            _actions.Disable();
            _actions.Dispose();
        }
    }
}
