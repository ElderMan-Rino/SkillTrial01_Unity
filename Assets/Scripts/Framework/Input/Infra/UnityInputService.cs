using Elder.Framework.Common.Base;
using Elder.Framework.Input.Domain.Values;
using Elder.Framework.Input.Infra.Generated;
using Elder.Framework.Input.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elder.Framework.Input.Infra
{
    internal sealed class UnityInputService : DisposableBase, IInputService
    {
        private readonly GameInputActions _inputActions;

        public UnityInputService(GameInputActions inputActions)
        {
            _inputActions = inputActions;
            _inputActions.Enable();
        }

        public AxisInputData GetAxisData()
        {
            var moveAction = _inputActions.FindAction("Move");
            var lookAction = _inputActions.FindAction("Look");

            var move = moveAction is not null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
            var look = lookAction is not null ? lookAction.ReadValue<Vector2>() : Vector2.zero;

            return new AxisInputData(
                new InputVector2(move.x, move.y),
                new InputVector2(look.x, look.y));
        }

        public bool GetButtonDown(string action)
        {
            var inputAction = _inputActions.FindAction(action);
            return inputAction is not null && inputAction.WasPressedThisFrame();
        }

        public bool GetButton(string action)
        {
            var inputAction = _inputActions.FindAction(action);
            return inputAction is not null && inputAction.IsPressed();
        }

        public bool GetButtonUp(string action)
        {
            var inputAction = _inputActions.FindAction(action);
            return inputAction is not null && inputAction.WasReleasedThisFrame();
        }

        protected override void DisposeManagedResources()
        {
            _inputActions.Disable();
            base.DisposeManagedResources();
        }
    }
}
