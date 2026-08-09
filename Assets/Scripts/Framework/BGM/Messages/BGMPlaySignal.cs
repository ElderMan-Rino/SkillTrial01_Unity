using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Bgm.Messages
{
    public readonly struct BGMPlaySignal : ISignal
    {
        public readonly string BGMKey;

        public BGMPlaySignal(string bgmKey)
        {
            BGMKey = bgmKey;
        }
    }
}
