using Cysharp.Threading.Tasks;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataSheetLoader
    {
        public UniTask LoadSheetAsync<T>(string assetName) where T : unmanaged;
    }
}