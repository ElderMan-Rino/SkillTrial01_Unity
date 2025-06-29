using Elder.Framework.Timer.Components;
using Elder.Framework.Timer.Definitions;
using Elder.Framework.Timer.Interfaces;
using Unity.Entities;

namespace Elder.Framework.Timer
{
    internal sealed class TimerService : ITimerService
    {
        private EntityManager _em;

        public TimerService()
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public TimerHandle Create(float duration, bool loop = false)
        {
            var entity = _em.CreateEntity(typeof(TimerComponent));
            _em.SetComponentData(entity, new TimerComponent
            {
                Duration = duration,
                Loop = loop,
                IsActive = false
            });
            return new TimerHandle(entity);
        }

        public void Start(TimerHandle handle) => SetActive(handle, true);
        public void Stop(TimerHandle handle) => SetActive(handle, false);

        public void Reset(TimerHandle handle)
        {
            if (!handle.IsValid) return;
            var t = _em.GetComponentData<TimerComponent>(handle.Entity);
            t.Elapsed = 0f;
            t.IsFinished = false;
            t.IsActive = false;
            _em.SetComponentData(handle.Entity, t);
        }

        public void Destroy(TimerHandle handle)
        {
            if (handle.IsValid) _em.DestroyEntity(handle.Entity);
        }

        public bool IsFinished(TimerHandle handle)
        {
            if (!handle.IsValid) return false;
            return _em.GetComponentData<TimerComponent>(handle.Entity).IsFinished;
        }

        public float GetElapsed(TimerHandle handle)
        {
            if (!handle.IsValid) return 0f;
            return _em.GetComponentData<TimerComponent>(handle.Entity).Elapsed;
        }

        private void SetActive(TimerHandle handle, bool active)
        {
            if (!handle.IsValid) return;
            var t = _em.GetComponentData<TimerComponent>(handle.Entity);
            t.IsActive = active;
            _em.SetComponentData(handle.Entity, t);
        }
    }
}
