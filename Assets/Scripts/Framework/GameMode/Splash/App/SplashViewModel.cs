using Elder.Framework.Common.Base;
using Elder.Framework.GameMode.Splash.Definitions;
using Elder.Framework.GameMode.Splash.Interfaces;

namespace Elder.Framework.GameMode.Splash.App
{
    internal sealed class SplashViewModel : DisposableBase, ISplashViewModel
    {
        private SplashEntry[] _entries = System.Array.Empty<SplashEntry>();

        public SplashEntry[] Entries => _entries;

        public void SetEntries(SplashEntry[] entries) => _entries = entries;

        public override void PreDispose() { }

        protected override void DisposeManagedResources()
        {
            _entries = System.Array.Empty<SplashEntry>();
        }
    }
}
