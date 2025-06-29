using Elder.Framework.Core.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Blob.Interfaces
{
    public interface IBlobIndexProvider : ISystemComponent
    {
        public bool TryGetIndex<TRoot>(int id, out int index);
        public void BuildIndex<TRoot>(IReadOnlyList<int> ids);
    }
}
