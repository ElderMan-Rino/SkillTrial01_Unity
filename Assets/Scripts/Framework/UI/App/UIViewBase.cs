using Cysharp.Threading.Tasks;
using Elder.Framework.Common.Base;
using Elder.Framework.UI.Interfaces;

namespace Elder.Framework.UI.App
{
    public abstract class UIViewBase : BehaviourBase, IUIView
    {
        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);
        public virtual void Refresh() { }

        public virtual UniTask OnShowAsync()
        {
            Show();
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnHideAsync()
        {
            Hide();
            return UniTask.CompletedTask;
        }
    }
}
