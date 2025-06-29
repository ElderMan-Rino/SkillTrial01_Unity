using Elder.Framework.Core;
using Elder.Framework.MonoEvent.Interfaces;
using Elder.Framework.Time.Definitions;
using Elder.Framework.Time.Domain;
using Elder.Framework.Time.Interfaces;
using R3;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Time.App
{
    internal sealed class TimerService : BaseSystem, ITimerService
    {
        private uint _nextId = 1;

        // [HEAP] Dictionary + List: 초기화 시 1회 할당
        private readonly Dictionary<uint, TimerEntry> _timers = new();
        private readonly List<uint> _toRemove = new();

        public override bool TryInitialize()
        {
            if (!TryGetSystem<IMonoEventBus>(out var bus)) return false;
            // [HEAP] Subscribe: 구독 객체 1회 할당 — AddTo로 Disposables에 등록
            bus.OnUpdate.Subscribe(_ => Tick()).AddTo(_disposables);
            return true;
        }

        public TimerHandle StartCountdown(float duration, IGameClock clock, Action onComplete = null)
            => CreateEntry(TimerMode.Countdown, duration, clock, onComplete);

        public TimerHandle StartInterval(float interval, IGameClock clock, Action onTick = null)
            => CreateEntry(TimerMode.Interval, interval, clock, onTick);

        public TimerHandle StartStopwatch(IGameClock clock)
            => CreateEntry(TimerMode.Stopwatch, 0f, clock, null);

        public void Cancel(TimerHandle handle)
        {
            if (!handle.IsValid) return;
            _timers.Remove(handle.Id);
        }

        public void CancelAll() => _timers.Clear();

        public float GetElapsed(TimerHandle handle)
        {
            if (_timers.TryGetValue(handle.Id, out var entry)) return entry.Elapsed;
            return 0f;
        }

        public float GetRemaining(TimerHandle handle)
        {
            if (_timers.TryGetValue(handle.Id, out var entry)) return entry.Remaining;
            return 0f;
        }

        public bool IsRunning(TimerHandle handle)
        {
            if (_timers.TryGetValue(handle.Id, out var entry)) return entry.IsRunning;
            return false;
        }

        private void Tick()
        {
            foreach (var pair in _timers)
            {
                pair.Value.Tick();
                // Countdown이 완료된 항목 수집
                if (!pair.Value.IsRunning && pair.Value.Mode == TimerMode.Countdown)
                    _toRemove.Add(pair.Key);
            }

            for (int i = 0; i < _toRemove.Count; i++)
                _timers.Remove(_toRemove[i]);

            _toRemove.Clear();
        }

        private TimerHandle CreateEntry(TimerMode mode, float duration, IGameClock clock, Action callback)
        {
            var handle = new TimerHandle(_nextId++);
            // [HEAP] TimerEntry: 타이머 1개당 1회 할당
            _timers[handle.Id] = new TimerEntry
            {
                Handle = handle,
                Clock = clock,
                Mode = mode,
                Duration = duration,
                Elapsed = 0f,
                IsRunning = true,
                OnComplete = callback,
            };
            return handle;
        }

        protected override void OnDispose()
        {
            _timers.Clear();
            _toRemove.Clear();
        }
    }
}
