using Elder.Framework.Core.Interfaces;
using Elder.Game.SkillTrial.GameMode.App;

namespace Elder.Game.SkillTrial.GameMode.Installer
{
    public readonly struct GameModeInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<SkillTrialGameModeInitializer>();
        }
    }
}