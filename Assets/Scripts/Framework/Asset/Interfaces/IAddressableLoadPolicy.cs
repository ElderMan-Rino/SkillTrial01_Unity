using Elder.Framework.Asset.Definitions;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAddressableLoadPolicy : IGameSystem
    {
        public bool TryResolve(string key, out PlatformAddressableRule rule);
    }
}
