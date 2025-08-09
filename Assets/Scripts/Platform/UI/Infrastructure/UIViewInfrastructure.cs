using UnityEngine;

namespace Elder.Platform.UI.Infrastructure
{
    public class UIViewInfrastructure : MonoBehaviour
    {
        private void Awake()
        {
            RegisterDontDestroyOnLoad();
        }
        private void RegisterDontDestroyOnLoad()
        {
            DontDestroyOnLoad(this);
        }
    }
}