using Elder.Framework.Core.Interfaces;
using Elder.Framework.Blob.Interfaces;
using Elder.SkillTrial.Resources.Data;

namespace Elder.SkillTrial.Data.Installer
{
    public readonly struct DataInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<GeneratedBlobLoader>().As<IGameDataLoader>();
            registry.Register<GeneratedBootstrapLoader>().As<IBootstrapDataLoader>();
        }
    }
}
