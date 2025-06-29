using Unity.Entities;

namespace Elder.Framework.Timer.Components
{
    public struct TimerComponent : IComponentData
    {
        public float Duration;
        public float Elapsed;
        public bool Loop;
        public bool IsActive;
        public bool IsFinished;
    }
}
