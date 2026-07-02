namespace Elder.Framework.GameMode.Splash.Definitions
{
    internal readonly struct SplashEntry
    {
        public readonly string Key;
        public readonly float Interval;

        public SplashEntry(string key, float interval)
        {
            Key = key;
            Interval = interval;
        }
    }
}
