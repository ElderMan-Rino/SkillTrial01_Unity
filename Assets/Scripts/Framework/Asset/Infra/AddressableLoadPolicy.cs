using Elder.Framework.Asset.Definitions;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core;
using UnityEngine;

namespace Elder.Framework.Asset.Infra
{
    internal sealed class AddressableLoadPolicy : BaseSystem, IAddressableLoadPolicy
    {
        private RuntimePlatform _currentPlatform;

        protected override void HandleInjectDependency()
        {
            _currentPlatform = Application.platform;
        }

        public bool TryResolve(string key, out PlatformAddressableRule rule)
        {
            // TODO: 데이터 툴(DataForge)에서 생성한 PlatformAddressableRuleTable 시트를 IDataProvider로 로드해 조회 예정
            // if (TryGetSystem<IDataProvider>(out var dataProvider))
            // {
            //     ref var table = ref dataProvider.GetBlobData<PlatformAddressableRuleTable>();
            //     for (int i = 0; i < table.Rules.Length; i++)
            //     {
            //         if (table.Rules[i].TargetPlatform != _currentPlatform) continue;
            //         if (table.Rules[i].Key != key) continue;
            //         rule = table.Rules[i];
            //         return true;
            //     }
            // }

            rule = default;
            return false;
        }
    }
}
