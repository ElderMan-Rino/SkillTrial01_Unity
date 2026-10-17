using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Root.Interfaces
{
    public interface IMainFrameworkRoot 
    {
        // 인터페이스에 dispose 관련을 추가해야함
        // 추가로 유니티 이벤트 받았을때 처리하는 부분들도 추가해야함
        public void ShutDown();
    }
}