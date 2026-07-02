using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Settings.Interfaces;
using Elder.Framework.Signal.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.Settings.Infra
{
    // ✅ OK: internal sealed — 구현 클래스 접근 수정자 준수
    // ✅ OK: 폴더 위치 — Infra/ (PlayerPrefs Unity API 어댑터)
    // ❌ VIOLATION: ISignalRouter _router 주입되나 전혀 사용되지 않음 — 불필요한 의존성 제거 필요
    internal sealed class PlayerPrefsSettingsStore : BaseSystem, IAppSettingsStore
    {
        private ISignalRouter _router;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<ISignalRouter>(out _router))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
        }

        public override UniTask InitializeAsync() => UniTask.CompletedTask;

        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;

        public bool TryGetString(string key, out string value, string defaultValue = "")
        {
            if (HasKey(key))
            {
                value = PlayerPrefs.GetString(key, defaultValue);
                return true;
            }
            value = defaultValue;
            return false;
        }

        public bool TryGetFloat(string key, out float value, float defaultValue = 0f)
        {
            if (HasKey(key))
            {
                value = PlayerPrefs.GetFloat(key, defaultValue);
                return true;
            }
            value = defaultValue;
            return false;
        }

        public bool TryGetInt(string key, out int value, int defaultValue = 0)
        {
            if (HasKey(key))
            {
                value = PlayerPrefs.GetInt(key, defaultValue);
                return true;
            }
            value = defaultValue;
            return false;
        }

        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);

        public bool HasKey(string key) => PlayerPrefs.HasKey(key);


    }
}
