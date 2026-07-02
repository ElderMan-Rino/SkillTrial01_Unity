using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Core;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Blob.App
{
    internal sealed class BlobIndexRegistry : BaseSystemComponent, IBlobIndexProvider
    {
        // [HEAP] 타입별 Dictionary 1회 할당 — 초기화 이후 read-only
        private readonly Dictionary<Type, Dictionary<int, int>> _indices = new();

        public bool TryGetIndex<TRoot>(int id, out int index)
        {
            if (_indices.TryGetValue(typeof(TRoot), out var map))
                return map.TryGetValue(id, out index);
            index = -1;
            return false;
        }

        public void BuildIndex<TRoot>(IReadOnlyList<int> ids)
        {
            // [HEAP] 타입당 1회 할당 — 이후 재할당 없음
            var map = new Dictionary<int, int>(ids.Count);
            for (int i = 0; i < ids.Count; i++)
                map[ids[i]] = i;
            _indices[typeof(TRoot)] = map;
        }
    }
}
