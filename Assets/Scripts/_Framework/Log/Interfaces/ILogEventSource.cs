using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Definitions;

namespace Elder.Framework.Log.Interfaces
{
    public interface ILogEventSource : IFrameworkComponent
    {
        public event LogEmitHandler OnLogPublished;
    }
}