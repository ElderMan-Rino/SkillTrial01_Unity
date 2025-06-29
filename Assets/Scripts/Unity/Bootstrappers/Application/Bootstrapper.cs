using Elder.Unity.CoreFrame.Infrastructure;
using UnityEngine;

namespace Elder.Unity.Bootstrappers.Application
{
    public static class Bootstrapper
    {
        private const string GameObjectName = "[MainFramework]";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Entry()
        {
            CreateMainFramework();
        }
        private static void CreateMainFramework()
        {
            if (GameObject.Find(GameObjectName))
                return;

            var go = new GameObject(GameObjectName);
            go.AddComponent<CoreFrameInfrastructure>();
        }
    }
}