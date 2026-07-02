using Elder.Framework.Common.Base;
using Elder.SkillTrial.GameMode.Preload.Loading.Definitions;
using Elder.SkillTrial.GameMode.Preload.Loading.Interfaces;

namespace Elder.SkillTrial.GameMode.Preload.Loading.App
{
    internal sealed class PreloadLoadingViewModel : DisposableBase, IPreloadLoadingViewModel
    {
        private PreloadLoadingSnapshot _snapshot;

        public PreloadLoadingSnapshot Snapshot => _snapshot;

        public void Apply(PreloadLoadingSnapshot snapshot) => _snapshot = snapshot;

        public override void PreDispose() { }

        protected override void DisposeManagedResources()
        {
            _snapshot = default;
        }
    }
}
