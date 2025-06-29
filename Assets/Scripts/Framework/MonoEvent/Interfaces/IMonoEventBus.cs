using Elder.Framework.Core.Interfaces;
using R3;

namespace Elder.Framework.MonoEvent.Interfaces
{
    public interface IMonoEventBus : IGameSystem
    {
        public Observable<float> OnUpdate { get; }
        public Observable<float> OnFixedUpdate { get; }
        public Observable<float> OnLateUpdate { get; }
        public Observable<bool> OnApplicationPause { get; }
        public Observable<Unit> OnApplicationQuit { get; }
    }
}
