using Unity.Entities;

namespace Elder.Framework.Timer.Definitions
{
    public readonly struct TimerHandle
    {
        public readonly Entity Entity;
        public TimerHandle(Entity entity) => Entity = entity;
        public bool IsValid => Entity != Entity.Null;
    }
}
