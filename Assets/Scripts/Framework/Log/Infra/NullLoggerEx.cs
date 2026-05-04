using Elder.Framework.Log.Interfaces;

namespace Elder.Framework.Log.Infra
{
    internal sealed class NullLoggerEx : ILoggerEx
    {
        internal static readonly NullLoggerEx Instance = new();

        public void Debug(string message) {}
        public void Info(string message) {}
        public void Warn(string message) {}
        public void Error(string message) {}
    }
}
