using Elder.Framework.Core.Interfaces;
using Elder.Framework.Time.Definitions;
using System;

namespace Elder.Framework.Time.Interfaces
{
    public interface ITimerService : IGameSystem
    {
        public TimerHandle StartCountdown(float duration, IGameClock clock, Action onComplete = null);
        public TimerHandle StartInterval(float interval, IGameClock clock, Action onTick = null);
        public TimerHandle StartStopwatch(IGameClock clock);

        public void Cancel(TimerHandle handle);
        public void CancelAll();

        public float GetElapsed(TimerHandle handle);
        public float GetRemaining(TimerHandle handle);
        public bool IsRunning(TimerHandle handle);

    }
}
