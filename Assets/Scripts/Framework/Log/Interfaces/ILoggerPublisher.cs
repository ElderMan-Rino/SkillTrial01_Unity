using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.Log.Interfaces
{
    public interface ILoggerPublisher : IGameSystem
    {
        public ILoggerEx GetLogger<T>() where T : class;
        public ILoggerEx GetLogger(Type type);
    }
}