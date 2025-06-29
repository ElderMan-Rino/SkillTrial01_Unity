namespace Elder.Framework.Log.Interfaces
{
    public interface ILoggerEx
    {
        public void Debug(string message);
        public void Info(string message);
        public void Warn(string message);
        public void Error(string message);
    }
}