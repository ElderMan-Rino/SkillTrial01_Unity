using Cysharp.Threading.Tasks;
using System.Threading;

namespace Elder.Framework.Core.Interfaces
{
    public interface IInitializable
    {
        public UniTask InitializeAsync(CancellationToken cancellationToken);
    }
}
