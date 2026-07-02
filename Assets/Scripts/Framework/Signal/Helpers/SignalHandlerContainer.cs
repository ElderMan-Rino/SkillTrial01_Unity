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
        // _tokenToIndex와 동기화된 핸들러 목록 — Publish 시 Dictionary.Values 열거자 힙 할당 제거
        private readonly List<SignalHandler<T>> _handlerValues = new();
        private long _lastTokenId;

        public void Publish(in T message)
        {
            // Publish 진입 시 count 캡처 — 순회 중 Add는 범위 밖, Remove(swap-remove)는 캡처 범위 내 인덱스만 영향
            int count = _handlerValues.Count;
            for (int i = 0; i < count; i++)
                _handlerValues[i]?.Invoke(in message);
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
        }

        public override void PreDispose() { }
    }
}