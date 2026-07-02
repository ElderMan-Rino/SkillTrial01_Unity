using Elder.Framework.Core.Interfaces;
using Elder.Game.SkillTrial.GameMode.Installer;

namespace Elder.Game.SkillTrial.Installer
{
    public readonly struct SkillTrialInstaller 
    {
        public void Install(IGameSystemRegistry registry)
        {
            new GameModeInstaller().Install(registry);
        }
    }
}