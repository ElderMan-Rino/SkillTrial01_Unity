using Elder.Framework.MessageBox.Definitions;
using Elder.Framework.Signal.Interfaces;
using System;

namespace Elder.Framework.MessageBox.Messages
{
    public readonly struct MessageBoxRequestSignal : ISignal
    {
        public readonly Guid RequestId;
        public readonly string Message;
        public readonly MessageBoxButtonType ButtonType;
        public readonly MessageBoxDismissPolicy DismissPolicy;

        public MessageBoxRequestSignal(Guid requestId, string message, MessageBoxButtonType buttonType, MessageBoxDismissPolicy dismissPolicy)
        {
            RequestId = requestId;
            Message = message;
            ButtonType = buttonType;
            DismissPolicy = dismissPolicy;
        }
    }
}
