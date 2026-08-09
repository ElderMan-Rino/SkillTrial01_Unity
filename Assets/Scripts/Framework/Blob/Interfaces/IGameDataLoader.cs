using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Blob.Interfaces
{
    public interface IGameDataLoader : ISystemComponent
    {
        public UniTask LoadAsync(IDataSheetLoader loader, int hash, int scope);
    }
}