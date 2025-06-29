using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Data.Interfaces
{
    public interface IGameDataLoader : IGameSystem
    {
        public UniTask LoadAsync(IDataSheetLoader loader, int hash);
    }
}