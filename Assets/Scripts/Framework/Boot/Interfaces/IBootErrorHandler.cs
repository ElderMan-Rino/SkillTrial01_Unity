using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Boot.Interfaces
{
    public interface IBootErrorHandler : IGameSystem
    {
        public UniTask HandleAsync(int errorCode);
    }
}
