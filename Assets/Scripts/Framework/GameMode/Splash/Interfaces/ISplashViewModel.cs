using Elder.Framework.GameMode.Splash.Definitions;

namespace Elder.Framework.GameMode.Splash.Interfaces
{
    internal interface ISplashViewModel
    {
        public SplashEntry[] Entries { get; }
        public void SetEntries(SplashEntry[] entries);
    }
}
