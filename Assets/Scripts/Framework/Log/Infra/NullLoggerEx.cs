using Elder.Framework.Log.Interfaces;

namespace Elder.Framework.Log.Infra
{
    internal sealed class NullLoggerEx : ILoggerEx
    {
        private static readonly NullLoggerEx _instance = new();
        internal static NullLoggerEx Instance => _instance;

        public void Debug(string message) {}
        public void Info(string message) {}
        public void Warn(string message) {}
        public void Error(string message) {}
    }
}
