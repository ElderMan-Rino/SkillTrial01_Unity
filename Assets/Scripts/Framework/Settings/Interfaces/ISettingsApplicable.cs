using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Settings.Interfaces
{
    public interface ISettingsApplicable : IGameSystem
    {
        public void ApplySettings(IAppSettingsStore store);
    }
}
