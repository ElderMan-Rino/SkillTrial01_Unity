/*
using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Infra.Extensions;
using Elder.Framework.Common.Utils;
using Elder.Framework.Core;
using Elder.Framework.Blob.Definitions;
using Elder.Framework.Blob.Interfaces;
using System.Threading;
using UnityEngine;

namespace Elder.SkillTrial.Resources.Data
{
    internal sealed class GeneratedBootstrapLoader : BaseSystem, IBootstrapDataLoader
    {
        private IGameDataLoader _dataLoader;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IGameDataLoader>(out _dataLoader))
                throw new System.InvalidOperationException($"[DI] Required system not found: {nameof(IGameDataLoader)}");
        }

        public async UniTask LoadBootstrapAsync(IDataSheetLoader loader, IDataProvider provider, string languageCode, CancellationToken ct)
        {
            Debug.Log($"[TEMP][Bootstrap] LoadBootstrapAsync 시작: languageCode={languageCode}"); // [HEAP] 문자열 보간

            ct.ThrowIfCancellationRequested();
            Debug.Log($"[TEMP][Bootstrap] BootstrapDataRoot 로드 시작: hash={SheetKey.BootstrapDataHash}"); // [HEAP] 문자열 보간
            await _dataLoader.LoadAsync(loader, SheetKey.BootstrapDataHash, DataScope.PersistentHash);
            Debug.Log("[TEMP][Bootstrap] BootstrapDataRoot 로드 완료");

            ct.ThrowIfCancellationRequested();
            if (!provider.TryGetBlobData<BootstrapDataRoot>(out var bootstrapData))
                throw new System.InvalidOperationException($"[Bootstrap] Required blob not found: {nameof(BootstrapDataRoot)}");
            int rowCount = bootstrapData.Rows.Length;
            Debug.Log($"[TEMP][Bootstrap] rowCount={rowCount}"); // [HEAP] 문자열 보간

            var tasks = new UniTask[rowCount + 2]; // [HEAP] 1회 할당
            for (int i = 0; i < rowCount; i++)
            {
                string dataKey = bootstrapData.Rows[i].DataKey.ToString(); // [HEAP] BlobString.ToString()
                int keyHash = StringHashHelper.ToStableHash(dataKey);
                Debug.Log($"[TEMP][Bootstrap] Row[{i}] DataKey='{dataKey}' → hash={keyHash}"); // [HEAP] 문자열 보간
                tasks[i] = _dataLoader.LoadAsync(loader, keyHash, DataScope.PersistentHash);
            }

            int localeHash = ResolveLocaleHash(languageCode);
            int errorLocaleHash = ResolveErrorLocaleHash(languageCode);
            Debug.Log($"[TEMP][Bootstrap] localeHash={localeHash} errorLocaleHash={errorLocaleHash} (languageCode={languageCode})"); // [HEAP] 문자열 보간
            tasks[rowCount] = _dataLoader.LoadAsync(loader, localeHash, DataScope.PersistentHash);
            tasks[rowCount + 1] = _dataLoader.LoadAsync(loader, errorLocaleHash, DataScope.PersistentHash);

            await UniTask.WhenAll(tasks); // [HEAP] WhenAll 내부 배열 참조
            Debug.Log("[TEMP][Bootstrap] LoadBootstrapAsync 전체 완료");
        }

        private static int ResolveLocaleHash(string languageCode) => languageCode switch
        {
            "Ko" => SheetKey.BootstrapLocale_KoHash,
            "Jp" => SheetKey.BootstrapLocale_JpHash,
            _ => SheetKey.BootstrapLocale_EnHash,
        };

        private static int ResolveErrorLocaleHash(string languageCode) => languageCode switch
        {
            "Ko" => SheetKey.ErrorMsgLocale_KoHash,
            "Jp" => SheetKey.ErrorMsgLocale_JpHash,
            _ => SheetKey.ErrorMsgLocale_EnHash,
        };
    }
}
*/
