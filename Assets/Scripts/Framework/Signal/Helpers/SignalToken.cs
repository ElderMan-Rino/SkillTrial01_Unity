using Elder.Framework.Signal.Interfaces;
using System;

namespace Elder.Framework.Signal.Helpers
{
    public struct SignalToken
    {
        private long _tokenId;
        private Type _messageType;
        private ISignalCancellable _signalCancellable;

        public SignalToken(long tokenId, Type messageType, ISignalCancellable signalCancellable)
        {
            _tokenId = tokenId;
            _messageType = messageType;
            _signalCancellable = signalCancellable;
        }

        public void Dispose()
        {
            _signalCancellable?.Unsubscribe(_messageType, _tokenId);
            _messageType = null;
            _signalCancellable = null;
        }

        public static SignalToken Empty { get; } = new SignalToken();
    }
}
