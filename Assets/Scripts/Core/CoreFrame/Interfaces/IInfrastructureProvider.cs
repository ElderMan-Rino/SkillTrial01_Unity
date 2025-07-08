using Elder.Core.Common.Interfaces;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IInfrastructureProvider
    {
        public bool TryGetInfrastructure<T>(out T targetInfrastructure) where T : class, IInfrastructure;
        /*
        → 인프라스트럭처는** 내부적으로 다른 인프라(인터페이스)**를 사용하는 것이 허용됩니다.
           즉, IInfrastructureProvider를 참조하는 것은 기술 구현 간 협력일 뿐이며 문제 없습니다.
           인프라 base에 IInfrastructureProvider를 주입
        */
    }
}
