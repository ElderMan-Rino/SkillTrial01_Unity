using Elder.Framework.UI.App;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Elder.TestModule.UITest.App
{
    // Popup 전용 테스트 View. UISystemTestView와 별개로 Open/Sleep/Wake/Close 흐름만 검증한다.
    internal sealed class UITestPopupView : UIViewBase
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        private readonly ReactiveCommand<Unit> _confirmClick = new(); // [HEAP]
        private readonly ReactiveCommand<Unit> _cancelClick = new(); // [HEAP]

        private VisualElement _root;
        private Button _confirmButton;
        private Button _cancelButton;

        public Observable<Unit> OnConfirmRequested => _confirmClick;
        public Observable<Unit> OnCancelRequested => _cancelClick;

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
            if (_confirmButton is not null)
            {
                UnsubscribeButtonEvents();
                _confirmButton = null;
                _cancelButton = null;
            }

            _root?.RemoveFromHierarchy();
            _root = null;
        }

        private void SetUIElements(VisualElement root)
        {
            _confirmButton = root.Q<Button>("confirm-popup-button");
            _cancelButton = root.Q<Button>("cancel-popup-button");
        }

        private void SubscribeButtonEvents()
        {
            _confirmButton.clicked += OnConfirmClicked;
            _cancelButton.clicked += OnCancelClicked;
        }

        private void UnsubscribeButtonEvents()
        {
            _confirmButton.clicked -= OnConfirmClicked;
            _cancelButton.clicked -= OnCancelClicked;
        }

        private void OnConfirmClicked()
        {
            _confirmClick.Execute(Unit.Default);
        }

        private void OnCancelClicked()
        {
            _cancelClick.Execute(Unit.Default);
        }

        private void OnDestroy()
        {
            if (_confirmButton is not null)
            {
                UnsubscribeButtonEvents();
            }

            _confirmClick.Dispose();
            _cancelClick.Dispose();
        }
    }
}
