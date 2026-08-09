using Cysharp.Threading.Tasks;
using Elder.Framework.Boot.Definitions;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Core;
using Elder.Framework.MessageBox.Definitions;
using Elder.Framework.MessageBox.Interfaces;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.Scene.Messages;
using Elder.Framework.Signal.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elder.Framework.Boot.App
{
    internal sealed class BootErrorHandler : BaseSystem, IBootErrorHandler
    {
        // [HEAP] 클래스 로드 시 1회 할당, 이후 조회만 수행
        private static readonly Dictionary<int, BootErrorPolicy> _policies = new()
        {
            [ErrorCode.Asset_LoadFailed] = new BootErrorPolicy("에셋을 불러오지 못했습니다.", MessageBoxButtonType.Ok, MessageBoxDismissPolicy.Fatal, BootErrorAction.QuitGame),
            [ErrorCode.Asset_AddressableInitFailed] = new BootErrorPolicy("에셋 시스템 초기화에 실패했습니다.", MessageBoxButtonType.Ok, MessageBoxDismissPolicy.Fatal, BootErrorAction.QuitGame),
            [ErrorCode.Blob_DeserializeFailed] = new BootErrorPolicy("데이터를 불러오지 못했습니다.", MessageBoxButtonType.Ok, MessageBoxDismissPolicy.Fatal, BootErrorAction.QuitGame),
            [ErrorCode.Blob_ChecksumMismatch] = new BootErrorPolicy("데이터가 손상되었습니다.", MessageBoxButtonType.Ok, MessageBoxDismissPolicy.Fatal, BootErrorAction.QuitGame),
            [ErrorCode.Bgm_AssetLoadFailed] = new BootErrorPolicy("배경음을 불러오지 못했습니다.", MessageBoxButtonType.Ok, MessageBoxDismissPolicy.Normal, BootErrorAction.Close),
            [ErrorCode.Localize_SheetLoadFailed] = new BootErrorPolicy("언어 데이터를 불러오지 못했습니다.", MessageBoxButtonType.Ok, MessageBoxDismissPolicy.Fatal, BootErrorAction.QuitGame),
            [ErrorCode.Scene_LoadFailed] = new BootErrorPolicy("씬을 불러오지 못했습니다.", MessageBoxButtonType.Ok, MessageBoxDismissPolicy.Fatal, BootErrorAction.NavigateToTitle),
        };

        private static readonly BootErrorPolicy _unknownPolicy = new("알 수 없는 오류가 발생했습니다.", MessageBoxButtonType.Ok, MessageBoxDismissPolicy.Fatal, BootErrorAction.QuitGame);

        private IMessageBoxCoordinator _messageBoxCoordinator;
        private ISignalRouter _router;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IMessageBoxCoordinator>(out _messageBoxCoordinator))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IMessageBoxCoordinator)}");
            if (!TryGetSystem<ISignalRouter>(out _router))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
        }

        public async UniTask HandleAsync(int errorCode)
        {
            var policy = _policies.TryGetValue(errorCode, out var found) ? found : _unknownPolicy;

            await _messageBoxCoordinator.RequestAsync(policy.Message, policy.ButtonType, policy.DismissPolicy);

            ExecuteAction(policy.Action);
        }

        private void ExecuteAction(BootErrorAction action)
        {
            switch (action)
            {
                case BootErrorAction.NavigateToTitle:
                    _router.Publish(new SceneTransitionSignal(SceneConstants.TitleSceneKey));
                    break;
                case BootErrorAction.QuitGame:
                    Application.Quit();
                    break;
                case BootErrorAction.Close:
                    break;
            }
        }
    }
}
