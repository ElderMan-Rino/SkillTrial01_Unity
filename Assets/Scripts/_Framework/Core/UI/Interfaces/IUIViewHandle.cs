using System;

namespace Elder.Framework.Core.UI.Interfaces
{
    // UIViewFactory가 생성한 View 인스턴스의 수명을 캡슐화하는 계약. 구현 타입으로의 캐스팅 금지(LSP).
    public interface IUIViewHandle : IDisposable
    {
        public IUIView View { get; }

        public void DestroyViewOnly();
    }
}
