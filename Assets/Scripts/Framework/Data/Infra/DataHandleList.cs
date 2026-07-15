using Elder.Framework.Blob.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Blob.App
{
    internal sealed class DataHandleList<T> : IDataHandleList where T : unmanaged
    {
        // [HEAP] 타입당 1회 할당
        private readonly List<IBlobDataHandle<T>> _handles = new();

        internal DataHandleList(int scope)
        {
            Scope = scope;
        }

        public int Scope { get; }
        internal int Count => _handles.Count;
        internal IBlobDataHandle<T> this[int i] => _handles[i];

        internal void Add(IBlobDataHandle<T> handle) => _handles.Add(handle);

        public void DisposeAll()
        {
            for (int i = 0; i < _handles.Count; i++)
                _handles[i].Dispose();
            _handles.Clear();
        }
    }
}
