using Unity.Entities;

namespace Elder.Framework.Asset.Definitions
{
    public struct PlatformAddressableRuleTable
    {
        public BlobArray<PlatformAddressableRule> Rules;
    }

    // Key: 코드가 참조하는 논리 이름 (플랫폼 불문 고정)
    // TargetPlatform: 같은 Key라도 플랫폼별로 행을 분리해 다른 Label을 매핑
    // AddressableLabel: RegisterAsync<T>(label)로 그룹 로드할 대상 (Local/Remote는 플랫폼별 빌드에서 Profile/Group으로 이미 고정되어 런타임에서 신경 쓸 필요 없음)
}
