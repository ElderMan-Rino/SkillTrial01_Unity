using Elder.Framework.Boot.Interfaces;
using UnityEngine;

namespace Elder.Framework.MainFrame.Infra.Configs
{
    // ❌ VIOLATION: ScriptableObject 사용 — 프로젝트 규칙상 보안 이슈로 금지 (feedback_no_scriptableobject.md)
      //   제안: MessagePack 직렬화 구조체 + ECS Blob 패턴으로 교체 필요
    // ✅ OK: 폴더 위치 — Infra/Configs/ (Unity 설정 파일 어댑터)
    [CreateAssetMenu(fileName = "FrameworkSettings", menuName = "Elder/Framework/FrameworkSettings")]
    public sealed class FrameworkSettings : ScriptableObject, IBootConfig
    {
        [field: SerializeField] public string BaseDataKey { get; private set; }

        [Tooltip("암호화 키 조각B. EditorEncryptionKeyConfig.KeyPartB와 동일한 값을 입력하세요.")]
        [field: SerializeField] public string EncryptionKeyPartB { get; private set; }
    }
}