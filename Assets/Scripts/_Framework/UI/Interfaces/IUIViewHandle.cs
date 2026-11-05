using System;

namespace Elder.Framework.UI.Interfaces
{
    // UIViewFactory가 생성한 View 인스턴스의 수명을 캡슐화하는 계약. 구현 타입으로의 캐스팅 금지(LSP).
    public interface IUIViewHandle : IDisposable
    {
        public IUIView View { get; }

        // Sleep 중에도 View가 null이 되어도 유지되는 타입 식별자. 동일 ViewType 중복 개수 집계 등 진단용.
        public Type ViewType { get; }

        public void DestroyViewOnly();

        // Sleep 상태에서 복귀 시 캐시된 IAssetHandle의 prefab을 Instantiate만 재수행 (GetAssetAsync 재호출 없음 = refCount 불변).
        public void Recreate();
    }
}
