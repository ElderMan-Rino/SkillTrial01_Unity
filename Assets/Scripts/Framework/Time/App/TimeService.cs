using Elder.Framework.Core;
using Elder.Framework.MonoEvent.Interfaces;
using Elder.Framework.Time.Domain;
using Elder.Framework.Time.Interfaces;
using R3;
using System.Collections.Generic;

namespace Elder.Framework.Time.App
{
    internal sealed class TimeService : BaseSystem, ITimeService
    {
        private readonly GameClock _global = new();
        private readonly GameClock _inGame;
        private readonly GameClock _ui;

        // [HEAP] Dictionary: 동적 Clock 관리용, 초기화 시 1회 할당
        private readonly Dictionary<string, GameClock> _dynamicClocks = new();

        // InGame은 Global 자식, UI는 Global 자식이나 InGame과 독립
        public TimeService()
        {
            _inGame = new GameClock(_global);
            _ui = new GameClock(_global);
        }

        public override bool TryInitialize()
        {
            if (!TryGetSystem<IMonoEventBus>(out var bus)) return false;
            // [HEAP] Subscribe: 구독 객체 1회 할당 — AddTo로 Disposables에 등록
            bus.OnUpdate.Subscribe(Tick).AddTo(_disposables);
            return true;
        }

        public IGameClock Global => _global;
        public IGameClock InGame => _inGame;
        public IGameClock UI => _ui;

        public IGameClock CreateClock(string id, IGameClock parent = null)
        {
            if (_dynamicClocks.TryGetValue(id, out var existing)) return existing;

            // parent가 GameClock이 아닌 경우 InGame을 기본 부모로 사용
            var parentClock = parent as GameClock ?? _inGame;
            var clock = new GameClock(parentClock); // [HEAP] 동적 Clock 생성
            _dynamicClocks[id] = clock;
            return clock;
        }

        public bool TryGetClock(string id, out IGameClock clock)
        {
            bool found = _dynamicClocks.TryGetValue(id, out var gameClock);
            clock = gameClock;
            return found;
        }

        public void ReleaseClock(string id) => _dynamicClocks.Remove(id);

        public void ReleaseAllDynamicClocks() => _dynamicClocks.Clear();

        private void Tick(float unscaledDeltaTime)
        {
            _global.Tick(unscaledDeltaTime);
            _inGame.Tick(unscaledDeltaTime);
            _ui.Tick(unscaledDeltaTime);

            foreach (var pair in _dynamicClocks)
                pair.Value.Tick(unscaledDeltaTime);
        }

        protected override void OnDispose()
        {
            _dynamicClocks.Clear();
        }
    }
}
