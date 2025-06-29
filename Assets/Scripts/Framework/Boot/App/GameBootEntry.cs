using Elder.Framework.MainFrame;
using Elder.Framework.MainFrame.Infra.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Elder.Framework.Boot.App
{
    internal static class GameBootEntry
    {
        private const string FrameworkSettingsKey = "FrameworkSettings";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            var handle = Addressables.LoadAssetAsync<FrameworkSettings>(FrameworkSettingsKey);
            // BeforeSceneLoad 단계에서는 비동기 불가 — WaitForCompletion으로 동기 처리
            var settings = handle.WaitForCompletion();

            if (settings is null)
            {
                Addressables.Release(handle);
                Debug.LogError($"[GameBootEntry] Addressables key '{FrameworkSettingsKey}' not found.");
                return;
            }

            var go = new GameObject(nameof(FrameworkRoot));
            var root = go.AddComponent<FrameworkRoot>();
            Addressables.Release(handle);
            root.Initialize(settings);
        }
    }
}
