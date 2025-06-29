using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Data.Interfaces;
using System.Collections.Generic;

namespace Elder.SkillTrial.Resources.Data
{
    internal sealed class GeneratedBlobLoader : BaseSystem, IGameDataLoader
    {
        public UniTask LoadAsync(IDataSheetLoader sheetLoader, int hash)
        {
            return GeneratedBlobRegistry.Registry.TryGetValue(hash, out var load)
                ? load(sheetLoader)
                : throw new KeyNotFoundException(GeneratedBlobRegistry.HashToKey.TryGetValue(hash, out var key) ? key : hash.ToString()); // [HEAP] error path only
        }
    }
}
