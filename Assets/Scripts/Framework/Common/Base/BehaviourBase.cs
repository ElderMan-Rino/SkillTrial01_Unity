using System;
using UnityEngine;

namespace Elder.Framework.Common.Base
{
    public abstract class BehaviourBase : MonoBehaviour, IDisposable
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

            OnDisposing();

            if (disposing)
                DisposeManagedResources();

            DisposeUnmanagedResources();
            FinalizeDispose();
            _disposed = true;
        }

        /// <summary>[Dispose 단계 1] 리소스 해제 전 중단 작업 훅.</summary>
        protected virtual void OnDisposing() { }

        /// <summary>[Dispose 단계 2] 관리 리소스(이벤트, 리스트, 자식 Dispose) 해제.</summary>
        protected virtual void DisposeManagedResources() { }

        /// <summary>[Dispose 단계 3] 비관리 리소스(네이티브 핸들 등) 해제.</summary>
        protected virtual void DisposeUnmanagedResources() { }

        /// <summary>[Dispose 단계 4] 모든 해제 완료 후 정리 로깅 등 후처리.</summary>
        protected virtual void FinalizeDispose() { }

        private void OnDestroy()
        {
            Dispose();
        }

        ~BehaviourBase()
        {
            Dispose(disposing: false);
        }
    }
}
