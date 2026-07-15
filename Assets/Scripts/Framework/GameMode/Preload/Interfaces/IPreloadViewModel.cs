using Elder.Framework.GameMode.Preload.Definitions;

namespace Elder.Framework.GameMode.Preload.Interfaces
{
    internal interface IPreloadViewModel
    {
        public PreloadEntry[] Entries { get; }
        public PreloadLoadingSnapshot Snapshot { get; }
        public void SetEntries(PreloadEntry[] entries);
        public void ApplySnapshot(PreloadLoadingSnapshot snapshot);
    }
}
