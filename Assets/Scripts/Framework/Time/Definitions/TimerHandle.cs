namespace Elder.Framework.Time.Definitions
{
    public readonly struct TimerHandle
    {
        public static readonly TimerHandle Invalid = new(0);

        internal readonly uint Id;

        internal TimerHandle(uint id) => Id = id;

        public bool IsValid => Id != 0;

        public override bool Equals(object obj) => obj is TimerHandle other && Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();
        public static bool operator ==(TimerHandle a, TimerHandle b) => a.Id == b.Id;
        public static bool operator !=(TimerHandle a, TimerHandle b) => a.Id != b.Id;
    }
}
