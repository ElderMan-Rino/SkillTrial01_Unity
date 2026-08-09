using Elder.Framework.MessageBox.Definitions;

namespace Elder.Framework.Boot.Definitions
{
    public readonly struct BootErrorPolicy
    {
        public readonly string Message;
        public readonly MessageBoxButtonType ButtonType;
        public readonly MessageBoxDismissPolicy DismissPolicy;
        public readonly BootErrorAction Action;

        public BootErrorPolicy(string message, MessageBoxButtonType buttonType, MessageBoxDismissPolicy dismissPolicy, BootErrorAction action)
        {
            Message = message;
            ButtonType = buttonType;
            DismissPolicy = dismissPolicy;
            Action = action;
        }
    }
}
