using Cysharp.Threading.Tasks;

namespace Elder.Framework.Data.Interfaces
{
    public interface IGameDataLoader
    {
        public UniTask LoadAllAsync(IDataSheetLoader loader);
        public UniTask LoadAsync<T>(IDataSheetLoader loader, string key) where T : unmanaged;
    }
}