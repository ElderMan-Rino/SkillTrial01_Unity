using Elder.Framework.Data.Interfaces;
using Elder.SkillTrial.Resources.Data;
using VContainer;
using VContainer.Unity;

namespace Elder.SkillTrial.Data.Installer
{
    public readonly struct GameInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IGameDataLoader, GeneratedBlobLoader>(Lifetime.Singleton);
        }
    }
}