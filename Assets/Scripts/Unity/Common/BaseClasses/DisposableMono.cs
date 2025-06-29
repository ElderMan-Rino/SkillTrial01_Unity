using UnityEngine;

namespace Elder.Unity.Common.BaseClasses
{
    public abstract class DisposableMono : MonoBehaviour
    {
        private bool _disposed = false;

        protected virtual void OnDestroy()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                DisposeManagedResources();

            DisposeUnmanagedResources();
            CompleteDispose();
            _disposed = true;
        }

        protected virtual void CompleteDispose() { }

        /// <summary>
        /// IDisposable 등 관리되는 자원 해제
        /// </summary>
        protected abstract void DisposeManagedResources();

        /// <summary>
        /// NativeArray 등 네이티브 리소스 해제
        /// </summary>
        protected abstract void DisposeUnmanagedResources();
    }
}
