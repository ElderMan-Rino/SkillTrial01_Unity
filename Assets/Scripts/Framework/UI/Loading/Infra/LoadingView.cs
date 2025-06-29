using Elder.Framework.UI.Loading.Definitions;
using Elder.Framework.UI.Loading.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace Elder.Framework.UI.Loading.Infra
{
    // USS class names: loading-gauge--bar / circle / dots / pulse / radial
    [RequireComponent(typeof(UIDocument))]
    public sealed class LoadingView : MonoBehaviour, ILoadingView
    {
        private static readonly string[] GaugeUssClasses =
        {
            "loading-gauge--bar",
            "loading-gauge--circle",
            "loading-gauge--dots",
            "loading-gauge--pulse",
            "loading-gauge--radial",
        };

        private VisualElement _imageEl;
        private ProgressBar   _progressBar;
        private Label         _progressText;
        private VisualElement _gaugeContainer;
        private VisualElement _root;

        private IVideoPlayer _videoPlayer;

        private void Awake()
        {
            var uiRoot = GetComponent<UIDocument>().rootVisualElement;

            _root           = uiRoot.Q<VisualElement>("loading-root");
            _imageEl        = uiRoot.Q<VisualElement>("loading-image");
            _gaugeContainer = uiRoot.Q<VisualElement>("loading-gauge-container");
            _progressBar    = uiRoot.Q<ProgressBar>("loading-progress-bar");
            _progressText   = uiRoot.Q<Label>("loading-progress-text");
        }

        public void SetBackgroundImage(Sprite sprite)
        {
            _imageEl.style.backgroundImage = sprite is null
                ? StyleKeyword.None
                : new StyleBackground(sprite);
        }

        public void SetGaugeType(LoadingGaugeType gaugeType)
        {
            foreach (var cls in GaugeUssClasses)
                _gaugeContainer.RemoveFromClassList(cls);

            _gaugeContainer.AddToClassList(GaugeUssClasses[(int)gaugeType]);
        }

        public void SetProgress(float value)
        {
            _progressBar.value = value * 100f;
        }

        public void SetProgressText(string text)
        {
            _progressText.text = text;
        }

        public void SetVideoPlayer(IVideoPlayer player)
        {
            _videoPlayer?.Stop();
            _videoPlayer = player;
        }

        public void Show()
        {
            _imageEl.style.display = DisplayStyle.Flex;
            _root.style.display    = DisplayStyle.Flex;
        }

        public void ShowProgressOnly()
        {
            _imageEl.style.display = DisplayStyle.None;
            _root.style.display    = DisplayStyle.Flex;
        }

        public void Hide() => _root.style.display = DisplayStyle.None;

        private void OnDestroy()
        {
            _videoPlayer?.Dispose();
            _videoPlayer = null;
        }
    }
}
