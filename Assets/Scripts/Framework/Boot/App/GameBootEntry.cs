using Elder.Framework.MainFrame.Infra;
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
