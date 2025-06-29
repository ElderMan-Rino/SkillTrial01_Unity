using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Definitions;

namespace Elder.Framework.Log.Interfaces
{
    public interface ILogAdapter : IGameSystem
    {
        public void DispatchLogEvent(in LogEvent logEvent);
    }
}