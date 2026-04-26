using Elder.Framework.Data.Interfaces;
using UnityEngine;

namespace Elder.Framework.MainFrame.Infra.Configs
{
    [CreateAssetMenu(fileName = "FrameworkSettings", menuName = "Elder/Framework/FrameworkSettings")]
    public class FrameworkSettings : ScriptableObject, IDataConfig
    {
        [field: SerializeField] public string BaseDataKey { get; private set; }

        [Tooltip("암호화 키 조각B. EditorEncryptionKeyConfig.KeyPartB와 동일한 값을 입력하세요.")]
        [field: SerializeField] public string EncryptionKeyPartB { get; private set; }
    }
}