using Elder.Framework.Common.Utils;

namespace Elder.Framework.Data.Definitions
{
    public static class DataScope
    {
        public const string Persistent = "Persistent";
        public static readonly int PersistentHash = StringHashHelper.ToStableHash(Persistent);

        public const string Boot = "Boot";
        public static readonly int BootHash = StringHashHelper.ToStableHash(Boot);

        public const string Preload = "Preload";
        public static readonly int PreloadHash = StringHashHelper.ToStableHash(Preload);
    }
}
