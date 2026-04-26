#if UNITY_EDITOR
using UnityEngine;

namespace Elder.SkillTrial.Editor.Crypto
{
    // gitignore 대상 asset. 프로젝트 외부에서 관리 권장.
    // 메뉴: Elder/Crypto/Create Encryption Key Config
    [CreateAssetMenu(fileName = "EditorEncryptionKeyConfig", menuName = "Elder/Crypto/Editor Encryption Key Config")]
    public sealed class EditorEncryptionKeyConfig : ScriptableObject
    {
        [Tooltip("런타임 FrameworkSettings.EncryptionKeyPartB와 동일한 값이어야 합니다.")]
        [SerializeField] private string _keyPartB;

        public string KeyPartB => _keyPartB;

#if UNITY_EDITOR
        public void SetKeyPartB(string value) => _keyPartB = value;
#endif
    }
}
#endif
