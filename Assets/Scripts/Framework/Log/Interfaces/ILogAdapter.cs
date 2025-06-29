using Elder.Framework.Log.Definitions;

namespace Elder.Framework.Log.Interfaces
{
    public interface ILogAdapter 
    {
        public void DispatchLogEvent(in LogEvent logEvent);
    }
}