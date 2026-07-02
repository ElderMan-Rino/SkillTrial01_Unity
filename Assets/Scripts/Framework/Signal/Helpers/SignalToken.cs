using Elder.Framework.Signal.Interfaces;
using System;

namespace Elder.Framework.Signal.Helpers
{
    public readonly struct SignalToken
    {
        private readonly long _tokenId;
        private readonly Type _messageType;
        private readonly ISignalCancellable _router;

        public SignalToken(long tokenId, Type messageType, ISignalCancellable router)
        {
            _tokenId = tokenId;
            _messageType = messageType;
            _router = router;
        }

        public void Dispose()
        {
            if (_router is null) return;
            _router.Unsubscribe(_messageType, _tokenId);
        }

        public static SignalToken Empty { get; } = new SignalToken();
    }
}
