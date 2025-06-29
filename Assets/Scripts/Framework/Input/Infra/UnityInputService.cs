using Elder.Framework.Common.Base;
using Elder.Framework.Input.Domain.Values;
using Elder.Framework.Input.Infra.Generated;
using Elder.Framework.Input.Interfaces;
using UnityEngine;

namespace Elder.Framework.Input.Infra
{
    public class UnityInputService : DisposableBase, IInputService
    {
        private readonly GameInputActions _inputActions;

        public UnityInputService(GameInputActions inputAction)
        {
            _inputActions = inputAction;
        }

        public AxisInputData GetAxisData()
        {
            throw new System.NotImplementedException();
        }

        public bool GetButtonDown(string action)
        {
            throw new System.NotImplementedException();
        }
    }
}