using Elder.Framework.Timer.Components;
using Unity.Burst;
using Unity.Entities;

namespace Elder.Framework.Timer.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct TimerTickSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            new TimerTickJob { DeltaTime = dt }.ScheduleParallel();
        }
    }

    [BurstCompile]
    internal partial struct TimerTickJob : IJobEntity
    {
        public float DeltaTime;

        [BurstCompile]
        private void Execute(ref TimerComponent timer)
        {
            if (!timer.IsActive || timer.IsFinished) return;

            timer.Elapsed += DeltaTime;
            if (timer.Elapsed < timer.Duration) return;

            if (timer.Loop)
                timer.Elapsed -= timer.Duration;
            else
            {
                timer.Elapsed = timer.Duration;
                timer.IsFinished = true;
                timer.IsActive = false;
            }
        }
    }
}
