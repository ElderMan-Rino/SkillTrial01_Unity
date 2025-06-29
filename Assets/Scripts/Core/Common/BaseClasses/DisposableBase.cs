using System;

namespace Elder.Core.Common.BaseClasses
{
    public abstract class DisposableBase : IDisposable
    {
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                DisposeManagedResources();

            DisposeUnmanagedResources();
            CompleteDispose();
            _disposed = true;
        }
        protected virtual void CompleteDispose()
        {

        }
        /// <summary>
        /// 관리되는 리소스를 해제합니다.
        /// 예시:
        /// - IDisposable 인터페이스를 구현한 객체 (예: Stream, HttpClient, Timer 등)
        /// - 이벤트 구독 해제 (예: SomeEvent -= Handler)
        /// - Unity 객체 참조 정리 (필요시 null 처리 등)
        /// </summary>
        protected abstract void DisposeManagedResources();
        /// <summary>
        /// 비관리 리소스를 해제합니다.
        /// 예시:
        /// - 파일 핸들, 윈도우 핸들, 네이티브 포인터 해제
        /// - Marshal.AllocHGlobal()로 할당한 메모리 해제
        /// - Unity의 NativeArray.Dispose() 등 네이티브 리소스 해제
        /// 일반적으로 이 메서드는 직접 네이티브 API를 사용하지 않는 경우 비워둡니다.
        /// </summary>
        protected abstract void DisposeUnmanagedResources();

        ~DisposableBase()
        {
            Dispose(false);
        }
    }
}
