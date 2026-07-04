using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Data.Interfaces
{
    public interface IGameDataLoader : ISystemComponent
    {
        public UniTask LoadAsync(IDataSheetLoader loader, int hash, int scope);
    }
}