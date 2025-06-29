using Elder.Framework.Core.Interfaces;
using Elder.Framework.Data.Interfaces;
using Elder.SkillTrial.Resources.Data;

namespace Elder.SkillTrial.Data.Installer
{
    public readonly struct GameInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<IGameDataLoader, GeneratedBlobLoader>();
        }
    }
}