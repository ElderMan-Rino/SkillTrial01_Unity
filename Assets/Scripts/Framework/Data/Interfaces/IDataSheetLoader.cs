using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataSheetLoader : IGameSystem
    {
        public UniTask LoadSheetAsync<T>(string assetName, int scope) where T : unmanaged;
    }
}