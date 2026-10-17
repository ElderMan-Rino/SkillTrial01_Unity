/*
using Elder.Framework.MessageBox.Definitions;
using Elder.Framework.MessageBox.Interfaces;
using Elder.Framework.UI.App;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Elder.Framework.MessageBox.Infra
{
    internal sealed class MessageBoxView : UIViewBase, IMessageBoxView
    {
        private Label _desc;
        private Button _okButton;
        private Button _cancelButton;
        private ReactiveCommand<MessageBoxResult> _buttonClick;
        [SerializeField] private PanelRenderer _panelRenderer;

        // 흐름: Coordinator.RequestAsync → MessageBoxRequestSignal → (Presenter/VM 경유 예정) SetContent →
        //       사용자 클릭 → Answered → (TODO: ViewModel.Answer 경유) → MessageBoxResultSignal → Coordinator.HandleMessageBoxResult
        public Observable<MessageBoxResult> OnButtonClicked => _buttonClick;


        private void Awake()
        {
            InitializeButtonClickCommand();
            _panelRenderer.RegisterUIReloadCallback(HandlePanelReloaded);
        }

        private void InitializeButtonClickCommand()
        {
            _buttonClick = new();
        }

        private void HandlePanelReloaded(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            if (_okButton is not null)
            {
                UnsubscribeButtonEvents();
            }

            SetUIElements(rootElement);
            SubscribeToButtonEvents();
        }

        private void SubscribeToButtonEvents()
        {
            _okButton.clicked += HandleOkClicked;
            _cancelButton.clicked += HandleCancelClicked;
        }

        private void SetUIElements(VisualElement root)
        {
            _desc = root.Q<Label>(className: "messagebox-desc");
            _okButton = root.Q<Button>(className: "messagebox-ok-button");
            _cancelButton = root.Q<Button>(className: "messagebox-cancel-button");
        }

        public void SetContent(string desc, MessageBoxButtonType buttonType)
        {
            _desc.text = desc;
            _cancelButton.style.display = buttonType == MessageBoxButtonType.OkCancel ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void HandleOkClicked()
        {
            ExcuteButtonClickEvent(MessageBoxResult.Ok);
        }

        private void HandleCancelClicked()
        {
            ExcuteButtonClickEvent(MessageBoxResult.Cancel);
        }

        private void ExcuteButtonClickEvent(MessageBoxResult result)
        {
            _buttonClick.Execute(result);
        }

        protected override void DisposeManagedResources()
        {
            _panelRenderer.UnregisterUIReloadCallback(HandlePanelReloaded);
            UnsubscribeButtonEvents();
            DisposeButtonClickCommand();
            base.DisposeManagedResources();
        }

        private void DisposeButtonClickCommand()
        {
            _buttonClick.Dispose();
            _buttonClick = null;
        }

        private void UnsubscribeButtonEvents()
        {
            _okButton.clicked -= HandleOkClicked;
            _cancelButton.clicked -= HandleCancelClicked;
        }
    }
}

*/
