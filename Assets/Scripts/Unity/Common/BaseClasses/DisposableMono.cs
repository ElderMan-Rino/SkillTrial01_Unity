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
        /// IDisposable �� �����Ǵ� �ڿ� ����
        /// </summary>
        protected abstract void DisposeManagedResources();

        /// <summary>
        /// NativeArray �� ����Ƽ�� ���ҽ� ����
        /// </summary>
        protected abstract void DisposeUnmanagedResources();
    }
}
