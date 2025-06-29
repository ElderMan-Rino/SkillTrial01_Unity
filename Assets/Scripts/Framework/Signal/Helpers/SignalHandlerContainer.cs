using Elder.Framework.Common.Base;
using Elder.Framework.Signal.Definitions;
using Elder.Framework.Signal.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Signal.Helpers
{
    internal sealed class SignalHandlerContainer<T> : DisposableBase, ISignalHandler where T : struct, ISignal
    {
        // tokenId → index in _handlerValues (swap-remove O(1))
        private readonly Dictionary<long, int> _tokenToIndex = new();
        // index → tokenId (역방향 조회용, foreach 없이 O(1) swap 갱신)
        private readonly List<long> _indexToToken = new();
        // [HEAP] Publish 중 Subscribe/Unsubscribe 호출로 인한 컬렉션 변경 방지용 스냅샷 버퍼
        private readonly List<SignalHandler<T>> _publishSnapshot = new();
        // _tokenToIndex와 동기화된 핸들러 목록 — Publish 시 Dictionary.Values 열거자 힙 할당 제거
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
            int newIndex = _handlerValues.Count;
            _tokenToIndex[_lastTokenId] = newIndex;
            _indexToToken.Add(_lastTokenId);
            _handlerValues.Add(handler);
        }

        public void SetLastTokenId(long tokenId)
        {
            _lastTokenId = tokenId;
        }

        public void Remove(long handlerId)
        {
            if (!_tokenToIndex.Remove(handlerId, out int index)) return;

            int last = _handlerValues.Count - 1;
            if (index < last)
            {
                // swap-remove: 마지막 요소를 제거 위치로 이동
                _handlerValues[index] = _handlerValues[last];
                long movedToken = _indexToToken[last];
                _indexToToken[index] = movedToken;
                _tokenToIndex[movedToken] = index;
            }
            _handlerValues.RemoveAt(last);
            _indexToToken.RemoveAt(last);
        }

        protected override void DisposeManagedResources()
        {
            _tokenToIndex.Clear();
            _indexToToken.Clear();
            _handlerValues.Clear();
            _publishSnapshot.Clear();
            base.DisposeManagedResources();
        }
    }
}