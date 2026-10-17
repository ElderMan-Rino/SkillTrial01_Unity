/*
using Elder.Framework.Boot.Definitions;
using Elder.Framework.Boot.Infra;
using Elder.Framework.MainFrame.Infra;
using Elder.Framework.MainFrame.Infra.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Boot.App
{
    internal static class GameBootEntry
    {
        private const string FrameworkSettingsKey = "FrameworkSettings";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            Debug.Log("[TEMP] OnBeforeSceneLoad 진입");

            var bootMode = GetSceneBootMode();
            Debug.Log($"[TEMP] bootMode={bootMode}");
            if (bootMode == SceneBootMode.None)
            {
                Debug.Log("[TEMP] bootMode None — 부팅 중단");
                return;
            }

            var handle = Addressables.LoadAssetAsync<FrameworkSettings>(FrameworkSettingsKey);
            var settings = handle.WaitForCompletion();
            Debug.Log($"[TEMP] FrameworkSettings 로드 결과={(settings is null ? "null" : "OK")}");

            if (settings is null)
            {
                Addressables.Release(handle);
                Debug.LogError($"[GameBootEntry] Addressables key '{FrameworkSettingsKey}' not found.");
                return;
            }

            var go = new GameObject(nameof(FrameworkRoot));
            var root = go.AddComponent<FrameworkRoot>();
            Addressables.Release(handle);
            Debug.Log("[TEMP] FrameworkRoot.Initialize 호출 직전");
            root.Initialize(settings, bootMode);
            Debug.Log("[TEMP] FrameworkRoot.Initialize 호출 완료");
        }

        private static SceneBootMode GetSceneBootMode()
        {
            var activeScene = SceneManager.GetActiveScene();
            Debug.Log($"[TEMP] activeScene={activeScene.name} isLoaded={activeScene.isLoaded} rootCount={activeScene.rootCount} sceneCount={SceneManager.sceneCount}");
            var marker = Object.FindAnyObjectByType<SceneBootModeMarker>();
            return marker is null ? SceneBootMode.None : marker.Mode;
        }
    }
}
*/