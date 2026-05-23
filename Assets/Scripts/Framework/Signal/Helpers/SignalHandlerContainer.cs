using Elder.Framework.Common.Base;
using Elder.Framework.Signal.Definitions;
using Elder.Framework.Signal.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Signal.Helpers
{
    internal sealed class SignalHandlerContainer<T> : DisposableBase, ISignalHandler where T : struct, ISignal
    {
        private readonly Dictionary<long, SignalHandler<T>> _handlers = new();
        // [HEAP] Publish 중 Subscribe/Unsubscribe 호출로 인한 컬렉션 변경 방지용 스냅샷 버퍼
        private readonly List<SignalHandler<T>> _publishSnapshot = new();
        // _handlers와 동기화된 값 목록 — Publish 시 Dictionary.Values 열거자 힙 할당 제거
        private readonly List<SignalHandler<T>> _handlerValues = new();
        private long _lastTokenId;

        public void Publish(in T message)
        {
            _publishSnapshot.Clear();
            for (int i = 0; i < _handlerValues.Count; i++)
                _publishSnapshot.Add(_handlerValues[i]);

            for (int i = 0; i < _publishSnapshot.Count; i++)
                _publishSnapshot[i]?.Invoke(in message);
        }

        public void Add(SignalHandler<T> handler)
        {
            if (!_handlers.TryAdd(_lastTokenId, handler)) return;
            _handlerValues.Add(handler);
        }

        public void SetLastTokenId(long tokenId)
        {
            _lastTokenId = tokenId;
        }

        public void Remove(long handlerId)
        {
            if (!_handlers.Remove(handlerId, out var handler)) return;
            _handlerValues.Remove(handler);
        }

        protected override void DisposeManagedResources()
        {
            _handlers.Clear();
            _handlerValues.Clear();
            _publishSnapshot.Clear();
            base.DisposeManagedResources();
        }
    }
}