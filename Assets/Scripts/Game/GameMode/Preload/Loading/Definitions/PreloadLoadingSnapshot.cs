namespace Elder.SkillTrial.GameMode.Preload.Loading.Definitions
{
    public readonly struct PreloadLoadingSnapshot
    {
        public readonly int Current;
        public readonly int Total;
        public readonly string Label;   // [HEAP] string 참조

        public PreloadLoadingSnapshot(int current, int total, string label)
        {
            Current = current;
            Total = total;
            Label = label;
        }
    }
}
