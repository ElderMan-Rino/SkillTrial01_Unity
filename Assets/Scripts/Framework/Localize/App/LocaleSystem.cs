using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Localize.Interfaces;
using Elder.Framework.Settings.Definitions;
using Elder.Framework.Settings.Interfaces;
using Elder.Framework.Signal.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.Localize.App
{
    // ✅ OK: internal sealed — 구현 클래스 접근 수정자 준수
    // ✅ OK: 폴더 위치 — App/ (로케일 Use Case 서비스)
    // ❌ VIOLATION: ApplySettings 로직 오류 — TryGetString 실패 시 return이지만 주석은 ResolveSystemLanguage 호출 의도
    //   if (!store.TryGetString(...)) return; 가 선행되어 아래 ResolveSystemLanguage가 절대 실행되지 않음
    //   제안: else 분기로 ResolveSystemLanguage 호출하도록 수정
    // ❌ VIOLATION: Initialize()의 _router.Subscribe 주석 처리 — 언어 변경 신호 구독 비활성화
    // ❌ VIOLATION: ISignalRouter _router 주입되나 Subscribe 주석 처리로 실질적으로 미사용
    internal sealed class LocaleSystem : BaseSystem, ILocaleSystem, ISettingsApplicable
    {
        private string _languageCode;
        private ISignalRouter _router;

        public string GetLanguageCode() => _languageCode;

        public void SetLanguageCode(string languageCode) => _languageCode = languageCode;

        public void ApplySettings(IAppSettingsStore store)
        {
            if (!store.TryGetString(SettingsKeys.Locale, out _languageCode))
                _languageCode = ResolveSystemLanguage(Application.systemLanguage);
        }

        private static string ResolveSystemLanguage(SystemLanguage lang) => lang switch
        {
            SystemLanguage.Korean   => "Ko",
            SystemLanguage.Japanese => "Jp",
            _                       => "En",
        };

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<ISignalRouter>(out _router))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
        }

        public override UniTask InitializeAsync()
        {
            //_router.Subscribe<>()
            // 변경에 대한 처리 진행
            return UniTask.CompletedTask;
        }

        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;
    }
}
