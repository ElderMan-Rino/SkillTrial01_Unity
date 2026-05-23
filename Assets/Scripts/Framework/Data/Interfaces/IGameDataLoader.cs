using Cysharp.Threading.Tasks;

namespace Elder.Framework.Data.Interfaces
{
    public interface IGameDataLoader
    {
        public UniTask LoadAsync(IDataSheetLoader loader, int hash);
    }
}