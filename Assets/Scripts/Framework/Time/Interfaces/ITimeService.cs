using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Time.Interfaces
{
    public interface ITimeService : IGameSystem
    {
        public IGameClock Global { get; }
        public IGameClock InGame { get; }
        public IGameClock UI { get; }

        public IGameClock CreateClock(string id, IGameClock parent = null);
        public bool TryGetClock(string id, out IGameClock clock);
        public void ReleaseClock(string id);
        public void ReleaseAllDynamicClocks();

    }
}
