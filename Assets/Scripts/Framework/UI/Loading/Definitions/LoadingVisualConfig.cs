using Elder.Framework.UI.Loading.Interfaces;
using UnityEngine;

namespace Elder.Framework.UI.Loading.Definitions
{
    public readonly struct LoadingVisualConfig
    {
        public readonly Sprite BackgroundSprite;
        public readonly IVideoPlayer VideoPlayer;
        public readonly LoadingGaugeType GaugeType;

        public static readonly LoadingVisualConfig Default = default;

        public LoadingVisualConfig(Sprite backgroundSprite = null, IVideoPlayer videoPlayer = null, LoadingGaugeType gaugeType = LoadingGaugeType.Bar)
        {
            BackgroundSprite = backgroundSprite;
            VideoPlayer = videoPlayer;
            GaugeType = gaugeType;
        }
    }
}
