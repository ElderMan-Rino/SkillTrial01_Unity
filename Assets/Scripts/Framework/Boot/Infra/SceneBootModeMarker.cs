using Elder.Framework.Boot.Definitions;
using UnityEngine;

namespace Elder.Framework.Boot.Infra
{
    [DisallowMultipleComponent]
    internal sealed class SceneBootModeMarker : MonoBehaviour
    {
        [SerializeField] private SceneBootMode _mode;
        public SceneBootMode Mode => _mode;
    }
}
