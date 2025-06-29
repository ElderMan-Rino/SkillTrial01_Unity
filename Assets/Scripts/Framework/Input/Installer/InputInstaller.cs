using Elder.Framework.Core.Interfaces;
using Elder.Framework.Input.Infra;
using Elder.Framework.Input.Interfaces;

namespace Elder.Framework.Input.Installer
{
    public readonly struct InputInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            // [HEAP] InputActionCollectionAdapter + UnityInputService — 등록 시점 1회
            registry.RegisterInstance<IInputControlCenter>(new InputControlCenter());
            registry.TryGetRegistered<IInputControlCenter>(out var inputActions);
            registry.RegisterInstance<IInputService>(new UnityInputService(inputActions));
        }
    }
}
