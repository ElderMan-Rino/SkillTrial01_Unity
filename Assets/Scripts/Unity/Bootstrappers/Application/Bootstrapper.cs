using Elder.Unity.CoreFrame.Presentation;
using UnityEngine;

namespace Elder.Unity.Bootstrappers.Application
{
    public static class Bootstrapper
    {
        private const string GameObjectName = "[MainFramework]";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Entry()
        {
            CreateCoreFrameRunner();
        }
        private static void CreateCoreFrameRunner()
        {
            if (GameObject.Find(GameObjectName))
                return;

            var go = new GameObject(GameObjectName);
            go.AddComponent<CoreFrameRunner>();
        }
    }
}