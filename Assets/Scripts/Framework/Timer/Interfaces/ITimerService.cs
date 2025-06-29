using Elder.Framework.Core.Interfaces;
using Elder.Framework.Timer.Definitions;

namespace Elder.Framework.Timer.Interfaces
{
    public interface ITimerService : ISystemComponent
    {
        public TimerHandle Create(float duration, bool loop = false);
        public void Start(TimerHandle handle);
        public void Stop(TimerHandle handle);
        public void Reset(TimerHandle handle);
        public void Destroy(TimerHandle handle);
        public bool IsFinished(TimerHandle handle);
        public float GetElapsed(TimerHandle handle);
    }
}
