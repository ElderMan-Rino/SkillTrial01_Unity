using Elder.Framework.UI.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace Elder.Framework.UI.App
{
    public abstract class UIViewBase : MonoBehaviour, IUIView
    {
        public abstract void BindRoot(VisualElement parent);
        public abstract void UnbindRoot();

        // CloneTree()가 반환하는 TemplateContainer를 부모의 flex 흐름에서 분리해 전체 영역에 오버레이로 겹치게 한다.
        protected static void StretchToOverlay(VisualElement root)
        {
            root.style.position = Position.Absolute;
            root.style.left = 0;
            root.style.top = 0;
            root.style.right = 0;
            root.style.bottom = 0;
        }
    }
}
