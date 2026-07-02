using Elder.SkillTrial.GameMode.Preload.Loading.Definitions;

namespace Elder.SkillTrial.GameMode.Preload.Loading.Interfaces
{
    public interface IPreloadLoadingViewModel
    {
        public PreloadLoadingSnapshot Snapshot { get; }
        public void Apply(PreloadLoadingSnapshot snapshot);
    }
}
