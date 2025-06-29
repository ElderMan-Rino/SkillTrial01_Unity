using Elder.Framework.Time.Definitions;
using Elder.Framework.Time.Interfaces;
using System;

namespace Elder.Framework.Time.Domain
{
    internal sealed class TimerEntry
    {
        internal TimerHandle Handle;
        internal IGameClock Clock;
        internal TimerMode Mode;
        internal float Duration;
        internal float Elapsed;
        internal bool IsRunning;
        internal Action OnComplete;  // [HEAP] 1회 할당 후 재사용

        internal float Remaining => Mode == TimerMode.Stopwatch ? 0f : MathF.Max(0f, Duration - Elapsed);

        internal void Tick()
        {
            if (!IsRunning) return;

            Elapsed += Clock.DeltaTime;

            switch (Mode)
            {
                case TimerMode.Countdown:
                    if (Elapsed >= Duration)
                    {
                        Elapsed = Duration;
                        IsRunning = false;
                        OnComplete?.Invoke();
                    }
                    break;

                case TimerMode.Interval:
                    if (Elapsed >= Duration)
                    {
                        Elapsed -= Duration;
                        OnComplete?.Invoke();
                    }
                    break;

                case TimerMode.Stopwatch:
                    break;
            }
        }
    }
}
