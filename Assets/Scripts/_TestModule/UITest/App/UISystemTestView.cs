using Elder.Framework.UI.App;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Elder.TestModule.UITest.App
{
    // UISystem 씬 전용 View. 메인 컨트롤 패널은 항시 유지되며 하위 패널/팝업 열기, 전체 닫기(메인 제외), 최상위 닫기를 검증한다.
    internal sealed class UISystemTestView : UIViewBase
    {
        [SerializeField] private PanelRenderer _panelRenderer;

        private readonly ReactiveCommand<Unit> _openPanelClick = new(); // [HEAP]
        private readonly ReactiveCommand<Unit> _openPopupClick = new(); // [HEAP]
        private readonly ReactiveCommand<Unit> _closeAllClick = new(); // [HEAP]
        private readonly ReactiveCommand<Unit> _closeTopClick = new(); // [HEAP]

        private Button _openPanelButton;
        private Button _openPopupButton;
        private Button _closeAllButton;
        private Button _closeTopButton;
        private VisualElement _panelContainer;

        public Observable<Unit> OnOpenPanelRequested => _openPanelClick;
        public Observable<Unit> OnOpenPopupRequested => _openPopupClick;
        public Observable<Unit> OnCloseAllRequested => _closeAllClick;
        public Observable<Unit> OnCloseTopRequested => _closeTopClick;

        public VisualElement PanelContainer => _panelContainer;

        private void Awake()
        {
            _panelRenderer.RegisterUIReloadCallback(HandlePanelReloaded);
        }

        private void HandlePanelReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            if (_openPanelButton is not null)
            {
                UnsubscribeButtonEvents();
            }

            SetUIElements(rootElement);
            SubscribeButtonEvents();
        }

        private void SetUIElements(VisualElement root)
        {
            _openPanelButton = root.Q<Button>("open-panel-button");
            _openPopupButton = root.Q<Button>("open-popup-button");
            _closeAllButton = root.Q<Button>("close-all-button");
            _closeTopButton = root.Q<Button>("close-top-button");
            _panelContainer = root.Q<VisualElement>("panel-container");
        }

        private void SubscribeButtonEvents()
        {
            _openPanelButton.clicked += OnOpenPanelClicked;
            _openPopupButton.clicked += OnOpenPopupClicked;
            _closeAllButton.clicked += OnCloseAllClicked;
            _closeTopButton.clicked += OnCloseTopClicked;
        }

        private void UnsubscribeButtonEvents()
        {
            _openPanelButton.clicked -= OnOpenPanelClicked;
            _openPopupButton.clicked -= OnOpenPopupClicked;
            _closeAllButton.clicked -= OnCloseAllClicked;
            _closeTopButton.clicked -= OnCloseTopClicked;
        }

        private void OnOpenPanelClicked()
        {
            _openPanelClick.Execute(Unit.Default);
        }

        private void OnOpenPopupClicked()
        {
            _openPopupClick.Execute(Unit.Default);
        }

        private void OnCloseAllClicked()
        {
            _closeAllClick.Execute(Unit.Default);
        }

        private void OnCloseTopClicked()
        {
            _closeTopClick.Execute(Unit.Default);
        }


        private void OnDestroy()
        {
            _panelRenderer.UnregisterUIReloadCallback(HandlePanelReloaded);
            if (_openPanelButton is not null)
            {
                UnsubscribeButtonEvents();
            }

            _openPanelClick.Dispose();
            _openPopupClick.Dispose();
            _closeAllClick.Dispose();
            _closeTopClick.Dispose();
        }
    }
}
