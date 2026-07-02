using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Log.Definitions;
using Elder.Framework.Log.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Log.Infra
{
    // ✅ OK: internal sealed — 구현 클래스 접근 수정자 준수
    // ✅ OK: 폴더 위치 — Infra/ (로그 인프라 구현체)
    // ❌ VIOLATION: foreach (var loggerEX in _loggerContainer) — [HEAP] 주석 누락 (Dictionary 열거자)
    // ❌ VIOLATION: foreach (var adapater in _logAdapters) — 오타: adapater → adapter
    internal sealed class LogPublisher : BaseSystem, ILoggerPublisher
    {
        private readonly Dictionary<Type, LoggerEX> _loggerContainer = new();

        private List<ILogAdapter> _logAdapters = new();

        protected override void HandleInjectDependency()
        { 
            TryGetSystems<ILogAdapter>(ref _logAdapters);
        }

        public override UniTask InitializeAsync() => UniTask.CompletedTask;

        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;

        public ILoggerEx GetLogger<T>() where T : class
        {
            return GetLogger(typeof(T));
        }

        public ILoggerEx GetLogger(Type type)
        {
            if (!_loggerContainer.TryGetValue(type, out var targetLogger))
            {
                targetLogger = new LoggerEX(type, PublishLogEvent); // [HEAP] 타입당 1회
                _loggerContainer[type] = targetLogger;
            }
            return targetLogger;
        }

        private void PublishLogEvent(in LogEvent logEvent)
        {
            foreach (var adapter in _logAdapters)
                adapter.DispatchLogEvent(logEvent);
        }

        protected sealed override void DisposeManagedResources()
        {
            DisposeLoggerEXContainer();
            DisposeLogAdapters();
        }

        private void DisposeLogAdapters()
        {
            _logAdapters.Clear();
            _logAdapters = null;
        }

        private void DisposeLoggerEXContainer()
        {
            foreach (var loggerEX in _loggerContainer)  
                loggerEX.Value.Dispose();
            _loggerContainer.Clear();
        }
    }
}