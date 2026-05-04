using Elder.Framework.Common.Base;
using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Flux.Helpers
{
    internal sealed class MessageHandlerContainer<T> : DisposableBase, IMessageHandler where T : struct, IFluxMessage
    {
        private readonly Dictionary<long, MessageHandler<T>> _handlers = new();
        // [HEAP] Publish 중 Subscribe/Unsubscribe 호출로 인한 컬렉션 변경 방지용 스냅샷 버퍼
        private readonly List<MessageHandler<T>> _publishSnapshot = new();
        private long _lastTokenId;

        public void Publish(in T message)
        {
            _publishSnapshot.Clear();
            foreach (var handler in _handlers.Values)
                _publishSnapshot.Add(handler);

            for (int i = 0; i < _publishSnapshot.Count; i++)
                _publishSnapshot[i]?.Invoke(in message);
        }

        public void Add(MessageHandler<T> handler)
        {
            _handlers.TryAdd(_lastTokenId, handler);
        }

        public void SetLastTokenId(long tokenId)
        {
            _lastTokenId = tokenId;
        }

        public void Remove(long handlerId)
        {
            _handlers.Remove(handlerId);
        }

        protected override void DisposeManagedResources()
        {
            _handlers.Clear();
            _publishSnapshot.Clear();
            base.DisposeManagedResources();
        }
    }
}