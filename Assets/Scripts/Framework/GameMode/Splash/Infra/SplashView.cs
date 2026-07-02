using Cysharp.Threading.Tasks;
using Elder.Framework.UI.App;
using UnityEngine;
using UnityEngine.UIElements;

namespace Elder.Framework.GameMode.Splash.Infra
{
    internal sealed class SplashView : UIViewBase
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private float _fadeDuration = 1.5f;

        private VisualElement _splashImage;
        private VisualElement _overlay;

        private void Awake()
        {
            var root = _document.rootVisualElement;
            _splashImage = root.Q(className: "splash-image");
            _overlay = root.Q(className: "splash-overlay");
        }

        public void Refresh(Sprite sprite)
        {
            if (_splashImage is null || sprite is null) return;
            _splashImage.style.backgroundImage = new StyleBackground(sprite);
        }

        public async UniTask PlayFadeInAsync()
        {
            await UniTask.NextFrame();
            _overlay.style.opacity = 0f;
            await UniTask.Delay((int)(_fadeDuration * 1000));
        }

        public async UniTask PlayFadeOutAsync(Sprite nextSprite = null)
        {
            _overlay.style.opacity = 1f;
            await UniTask.Delay((int)(_fadeDuration * 1000));
            if (nextSprite is not null) Refresh(nextSprite);
        }
    }
}
