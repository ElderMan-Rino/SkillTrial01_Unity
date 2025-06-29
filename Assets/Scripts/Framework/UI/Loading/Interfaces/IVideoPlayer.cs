using Cysharp.Threading.Tasks;
using System;

namespace Elder.Framework.UI.Loading.Interfaces
{
    public interface IVideoPlayer : IDisposable
    {
        public bool IsPlaying { get; }

        public UniTask PlayAsync(string assetKey);
        public void Stop();
        public void SetVisible(bool visible);
    }
}
