using Elder.Framework.Data.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Data.App
{
    internal sealed class DataHandleList<T> : IDataHandleList where T : unmanaged
    {
        // [HEAP] 타입당 1회 할당
        private readonly List<IDataHandle<T>> _handles = new();

        internal DataHandleList(int scope)
        {
            Scope = scope;
        }

        public int Scope { get; }
        internal int Count => _handles.Count;
        internal IDataHandle<T> this[int i] => _handles[i];

        internal void Add(IDataHandle<T> handle) => _handles.Add(handle);

        public void DisposeAll()
        {
            for (int i = 0; i < _handles.Count; i++)
                _handles[i].Dispose();
            _handles.Clear();
        }
    }
}
