namespace Elder.Framework.Time.Interfaces
{
    public interface IGameClock
    {
        public float DeltaTime { get; }
        public float UnscaledDeltaTime { get; }
        public float ElapsedTime { get; }
        public float UnscaledElapsedTime { get; }
        public float TimeScale { get; }
        public bool IsPaused { get; }

        public void SetTimeScale(float scale);
        public void Pause();
        public void Resume();
    }
}
