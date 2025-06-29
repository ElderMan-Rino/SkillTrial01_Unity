using System;
using System.Collections.Generic;

namespace Elder.Framework.Common.Utils
{
    // 범용 관리형 Object Pool — class T 재사용 (GC 압력 제거)
    public sealed class ManagedObjectPool<T> where T : class
    {
        private readonly Stack<T>  _idle;
        private readonly Func<T>   _factory;
        private readonly Action<T> _onRent;
        private readonly Action<T> _onReturn;

        // [HEAP] Stack + 팩토리 1회 캡처
        public ManagedObjectPool(Func<T> factory, Action<T> onRent = null, Action<T> onReturn = null, int preWarm = 0)
        {
            _idle     = new Stack<T>(preWarm);
            _factory  = factory;
            _onRent   = onRent;
            _onReturn = onReturn;

            for (int i = 0; i < preWarm; i++)
                _idle.Push(_factory());
        }

        public T Rent()
        {
            var obj = _idle.Count > 0 ? _idle.Pop() : _factory(); // [HEAP] 풀 미스 시만
            _onRent?.Invoke(obj);
            return obj;
        }

        public void Return(T obj)
        {
            if (obj is null) return;
            _onReturn?.Invoke(obj);
            _idle.Push(obj);
        }

        public int IdleCount => _idle.Count;
    }
}
