using Elder.Framework.Common.Base;
using Elder.Framework.GameMode.Preload.Definitions;
using Elder.Framework.GameMode.Preload.Interfaces;

namespace Elder.Framework.GameMode.Preload.App
{
    internal sealed class PreloadViewModel : DisposableBase, IPreloadViewModel
    {
        private PreloadEntry[] _entries = System.Array.Empty<PreloadEntry>();
        private PreloadLoadingSnapshot _snapshot;

        public PreloadEntry[] Entries => _entries;
        public PreloadLoadingSnapshot Snapshot => _snapshot;

        public void SetEntries(PreloadEntry[] entries) => _entries = entries;
        public void ApplySnapshot(PreloadLoadingSnapshot snapshot) => _snapshot = snapshot;

        public override void PreDispose() { }

        protected override void DisposeManagedResources()
        {
            _entries = System.Array.Empty<PreloadEntry>();
            _snapshot = default;
        }
    }
}
