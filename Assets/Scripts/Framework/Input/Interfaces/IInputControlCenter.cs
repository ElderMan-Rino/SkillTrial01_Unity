using Elder.Framework.Core.Interfaces;
using System;
using UnityEngine.InputSystem;

namespace Elder.Framework.Input.Interfaces
{
    public interface IInputControlCenter : IGameSystem, IDisposable
    {
        public void Enable();
        public void Disable();
        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false);
    }
}
