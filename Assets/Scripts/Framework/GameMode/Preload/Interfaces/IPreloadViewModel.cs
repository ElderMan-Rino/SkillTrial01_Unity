using Elder.Framework.GameMode.Preload.Definitions;

namespace Elder.Framework.GameMode.Preload.Interfaces
{
    internal interface IPreloadViewModel
    {
        public PreloadEntry[] Entries { get; }
        public void SetEntries(PreloadEntry[] entries);
    }
}
