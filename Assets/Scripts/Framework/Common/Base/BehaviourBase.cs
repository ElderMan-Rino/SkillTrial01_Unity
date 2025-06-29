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

        /// <summary>
        /// [Dispose 순서 1] 본격적인 리소스 해제 전, 사전 정리 작업을 수행하는 훅(Hook)입니다.
        /// <para>호출 시점: Dispose가 시작된 직후, 자원이 해제되기 전에 가장 먼저 호출됩니다.</para>
        /// <para>수행 작업: 실행 중인 로직 중단(Stop), 상태 플래그 변경(IsRunning = false), 로깅 등.</para>
        /// </summary>
        protected virtual void OnDisposing() { }

        /// <summary>
        /// [Dispose 순서 2] 관리되는 자원(Managed Resources)을 해제합니다.
        /// <para>호출 시점: Dispose(true)가 호출되었을 때만 실행됩니다.</para>
        /// <para>수행 작업: 이벤트 구독 해제, 리스트/딕셔너리 Clear, 하위 객체 Dispose 호출 등.</para>
        /// </summary>
        protected virtual void DisposeManagedResources() { }

        /// <summary>
        /// [Dispose 순서 3] 비관리 자원(Unmanaged Resources)을 해제합니다.
        /// <para>호출 시점: Dispose(true/false) 여부와 상관없이 항상 실행됩니다.</para>
        /// <para>수행 작업: 네이티브 핸들 해제, C++ DLL 연결 해제 등.</para>
        /// </summary>
        protected virtual void DisposeUnmanagedResources() { }

        /// <summary>
        /// [Dispose 순서 4] 모든 리소스 해제 후, 최종 마무리를 수행합니다.
        /// <para>호출 시점: _disposed 플래그가 true가 되기 직전, 가장 마지막에 실행됩니다.</para>
        /// <para>수행 작업: 로그 출력, 최종 상태 보고 등.</para>
        /// </summary>
        protected virtual void FinalizeDispose() { }

        private void OnDestroy()
        {
            Dispose();
        }

        // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        ~BehaviourBase()
        {
            Dispose(disposing: false);
        }
    }
}