using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System.Threading;

namespace Elder.Framework.Data.Interfaces
{
    public interface IBootstrapDataLoader : ISystemComponent
    {
        public UniTask LoadBootstrapAsync(IDataSheetLoader loader, IDataProvider provider, string languageCode, CancellationToken ct);
    }
}
