namespace Elder.Framework.UI.Definitions
{
    public readonly struct UIStackOptions
    {
        public static readonly UIStackOptions Default = new(16);

        public readonly int MaxStackSize;

        public UIStackOptions(int maxStackSize)
        {
            MaxStackSize = maxStackSize < 1 ? 1 : maxStackSize;
        }
    }
}
