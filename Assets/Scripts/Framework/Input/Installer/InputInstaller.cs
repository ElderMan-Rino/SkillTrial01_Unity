using Elder.Framework.Core.Interfaces;
using Elder.Framework.Input.Infra;
using Elder.Framework.Input.Infra.Generated;
using Elder.Framework.Input.Interfaces;

namespace Elder.Framework.Input.Installer
{
    public readonly struct InputInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<GameInputActions, GameInputActions>();
            registry.RegisterFactory<IInputService>(p =>
            {
                p.TryResolve<GameInputActions>(out var inputActions);
                return new UnityInputService(inputActions);
            });
        }
    }
}