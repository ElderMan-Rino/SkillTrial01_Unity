using Elder.Framework.Input.Infra;
using Elder.Framework.Input.Infra.Generated;
using Elder.Framework.Input.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Input.Installer
{
    public readonly struct InputInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<GameInputActions>(Lifetime.Singleton);

            builder.Register<UnityInputService>(Lifetime.Singleton).As<IInputService>();
        }
    }
}