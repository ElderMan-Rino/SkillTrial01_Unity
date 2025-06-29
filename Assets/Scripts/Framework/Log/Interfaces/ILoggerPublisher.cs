using System;

namespace Elder.Framework.Log.Interfaces
{
    public interface ILoggerPublisher 
    {
        public ILoggerEx GetLogger<T>() where T : class;
        public ILoggerEx GetLogger(Type type);
    }
}