using Elder.Framework.Boot.Definitions;
using Elder.Framework.Boot.Infra;
using Elder.Framework.Core.UI.Infra;
using Elder.Framework.Root;
using Elder.Framework.Root.Infra;
using Elder.Framework.Signal.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Boot.App
{
    internal static class GameBootEntry
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            var bootMode = GetSceneBootMode();
            if (bootMode != SceneBootMode.RealFlow)
            {
                Debug.Log("[TEMP] bootMode RealFlow 아님 — GameBootEntry 부팅 중단");
                return;
            }

            var go = new GameObject();
            var anchor = go.AddComponent<MainFrameworkAnchor>();
            var root = new MainFrameworkRoot();
            anchor.BindRoot(root);
            root.Initialize();
        }

        private static SceneBootMode GetSceneBootMode()
        {
            var activeScene = SceneManager.GetActiveScene();
            var marker = Object.FindAnyObjectByType<SceneBootModeMarker>();
            return marker is null ? SceneBootMode.None : marker.Mode;
        }
    }
}