using Elder.Framework.Core.Interfaces;
using Elder.Game.SkillTrial.Installer;

namespace Elder.SkillTrial.Data.Installer
{
    public readonly struct GameInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            new SkillTrialInstaller().Install(registry);
            new DataInstaller().Install(registry);
        }
    }
}