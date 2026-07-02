using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Core;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Boot.Infra
{
    internal sealed class UnityStartupEnvironment : IStartupEnvironment
    {
        private const int InitSceneIndex = 0;

        public bool IsInitSceneActive()
        {
            return SceneManager.GetActiveScene().buildIndex == InitSceneIndex;
        }
    }
}