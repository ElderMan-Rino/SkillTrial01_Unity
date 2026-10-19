using Cysharp.Threading.Tasks;
using System.Threading;

namespace Elder.Framework.Core.Interfaces
{
    public interface IPostInitializable
    {
        public UniTask PostInitializeAsync(CancellationToken cancellationToken);
    }
}
