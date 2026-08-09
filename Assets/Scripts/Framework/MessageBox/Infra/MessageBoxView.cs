using Elder.Framework.MessageBox.Definitions;
using Elder.Framework.MessageBox.Interfaces;
using Elder.Framework.UI.App;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Elder.Framework.MessageBox.Infra
{
    internal sealed class MessageBoxView : UIViewBase, IMessageBoxView
    {
        [SerializeField] private UIDocument _document;

        private Label _messageLabel;
        private Button _okButton;
        private Button _cancelButton;

        // 흐름: Coordinator.RequestAsync → MessageBoxRequestSignal → (Presenter/VM 경유 예정) SetContent →
        //       사용자 클릭 → Answered → (TODO: ViewModel.Answer 경유) → MessageBoxResultSignal → Coordinator.HandleMessageBoxResult
        public event Action<MessageBoxResult> Answered;

        private void Awake()
        {
            var root = _document.rootVisualElement;
            _messageLabel = root.Q<Label>(className: "messagebox-message");
            _okButton = root.Q<Button>(className: "messagebox-ok-button");
            _cancelButton = root.Q<Button>(className: "messagebox-cancel-button");

            _okButton.clicked += HandleOkClicked;
            _cancelButton.clicked += HandleCancelClicked;
        }

        public void SetContent(string message, MessageBoxButtonType buttonType)
        {
            _messageLabel.text = message;
            _cancelButton.style.display = buttonType == MessageBoxButtonType.OkCancel ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void HandleOkClicked()
        {
            Answered?.Invoke(MessageBoxResult.Ok);
        }

        private void HandleCancelClicked()
        {
            Answered?.Invoke(MessageBoxResult.Cancel);
        }
    }
}
