using Elder.Framework.Data.Interfaces;
using UnityEngine;

namespace Elder.Framework.MainFrame.Infra.Configs
{
    [CreateAssetMenu(fileName = "FrameworkSettings", menuName = "Elder/Framework/FrameworkSettings")]
    public class FrameworkSettings : ScriptableObject, IDataConfig 
    {
        [field: SerializeField] public string BaseDataKey { get; private set; } 
    }
}