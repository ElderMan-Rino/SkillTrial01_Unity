using Elder.Framework.Boot.Definitions.Constants;
using Elder.Framework.Boot.Interfaces;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Boot.Infra.Unity
{
    public class UnityStartupEnvironment : IStartupEnvironment
    {
        public bool IsInitSceneActive()
        {
            return SceneManager.GetActiveScene().buildIndex == BootConstants.InitSceneIndex;
        }
    }
}