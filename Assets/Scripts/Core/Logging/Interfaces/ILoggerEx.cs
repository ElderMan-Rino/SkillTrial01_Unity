namespace Elder.Core.Logging.Interfaces
{
    public interface ILoggerEx
    {
        public void Debug(string message);
        public void Info(string message);
        public void Warning(string message);
        public void Error(string message);
    }
}
