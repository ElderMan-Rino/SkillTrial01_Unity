using Cysharp.Threading.Tasks;

namespace Elder.Framework.Data.Interfaces
{
    public interface IGameDataLoader
    {
        public UniTask LoadAsync<T>(IDataSheetLoader loader, string key) where T : unmanaged;
        public UniTask LoadByKeyAsync(IDataSheetLoader loader, string key);
        public UniTask LoadAllAsync(IDataSheetLoader loader, IDataProvider provider, string languageCode);
    }
}