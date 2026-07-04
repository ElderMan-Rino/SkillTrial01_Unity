using Elder.Framework.UI.App;
using UnityEngine;
using UnityEngine.UIElements;

namespace Elder.Framework.GameMode.Preload.Infra
{
    internal sealed class PreloadView : UIViewBase
    {
        [SerializeField] private UIDocument _document;

        private VisualElement _progressBar;

        private void Awake()
        {
            var root = _document.rootVisualElement;
            _progressBar = root.Q(className: "preload-progress-bar");
        }

        public void Refresh(float progress)
        {
            if (_progressBar is null) return;
            _progressBar.style.width = new StyleLength(new Length(progress * 100f, LengthUnit.Percent));
        }
    }
}
