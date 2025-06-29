using Elder.Framework.UI.Loading.Definitions;
using UnityEngine;

namespace Elder.Framework.UI.Loading.Interfaces
{
    public interface ILoadingView
    {
        public void SetBackgroundImage(Sprite sprite);
        public void SetGaugeType(LoadingGaugeType gaugeType);
        public void SetProgress(float value);
        public void SetProgressText(string text);
        public void SetVideoPlayer(IVideoPlayer player);
        public void Show();
        public void ShowProgressOnly();
        public void Hide();
    }
}
