using Elder.Framework.UI.App;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Elder.TestModule.UITest.App
{
    // Panel 전용 테스트 View. UISystemTestView와 별개로 Open/Sleep/Wake/Close 흐름만 검증한다.
    internal sealed class UITestPanelView : UIViewBase
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        private readonly ReactiveCommand<Unit> _closeClick = new(); // [HEAP]

        private VisualElement _root;
        private Button _closeButton;

        public Observable<Unit> OnCloseRequested => _closeClick;

        public override void BindRoot(VisualElement parent)
        {
            _root = _visualTreeAsset.CloneTree(); // [HEAP]
            StretchToOverlay(_root);
            SetUIElements(_root);
            SubscribeButtonEvents();
            parent.Add(_root);
        }

        public override void UnbindRoot()
        {
            if (_closeButton is not null)
            {
                UnsubscribeButtonEvents();
                _closeButton = null;
            }

            _root?.RemoveFromHierarchy();
            _root = null;
        }

        private void SetUIElements(VisualElement root)
        {
            _closeButton = root.Q<Button>("close-panel-button");
        }

        private void SubscribeButtonEvents()
        {
            _closeButton.clicked += OnCloseClicked;
        }

        private void UnsubscribeButtonEvents()
        {
            _closeButton.clicked -= OnCloseClicked;
        }

        private void OnCloseClicked()
        {
            _closeClick.Execute(Unit.Default);
        }

        private void OnDestroy()
        {
            if (_closeButton is not null)
            {
                UnsubscribeButtonEvents();
            }

            _closeClick.Dispose();
        }
    }
}
