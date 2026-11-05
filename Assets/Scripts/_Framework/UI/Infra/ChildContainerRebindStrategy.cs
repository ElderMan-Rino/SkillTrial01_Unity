using Elder.Framework.UI.Interfaces;
using UnityEngine.UIElements;

namespace Elder.Framework.UI.Infra
{
    internal sealed class ChildContainerRebindStrategy : IUIRebindStrategy
    {
        private readonly VisualElement _parent;

        public ChildContainerRebindStrategy(VisualElement parent)
        {
            _parent = parent;
        }

        public void Rebind(IUIView view)
        {
            view.BindRoot(_parent);
        }
    }
}
