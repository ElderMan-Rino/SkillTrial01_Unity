using Elder.Framework.Core.Interfaces;
using Elder.Framework.Root.Interfaces;
using UnityEngine;

namespace Elder.Framework.Root.Infra
{
    public sealed class MainFrameworkAnchor : MonoBehaviour
    {
        private IMainFrameworkRoot _root;
        private IComponentProvider _provider;

        public void BindRoot(IMainFrameworkRoot root)
        {
            _root = root;
            _provider = root as IComponentProvider;
        }

        public bool TryGetComponent<T>(out T target) where T : IFrameworkComponent
        {
            if (_provider is null)
            {
                target = default;
                return false;
            }

            return _provider.TryGetComponent(out target);
        }

        private void OnDestroy()
        {
            _root?.ShutDown();
            _root = null;
            _provider = null;
        }
    }
}