using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Infra.Extensions;
using Elder.Framework.Common.Utils;
using Elder.Framework.Core;
using Elder.Framework.Data.Interfaces;
using System.Threading;

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
            ct.ThrowIfCancellationRequested();
            await _dataLoader.LoadAsync(loader, SheetKey.BootstrapDataHash);

            ct.ThrowIfCancellationRequested();
            var blobRef = provider.GetBlobReference<BootstrapDataRoot>();
            int rowCount = blobRef.Value.Rows.Length;

            var tasks = new UniTask[rowCount + 2]; // [HEAP] 1회 할당
            for (int i = 0; i < rowCount; i++)
                tasks[i] = _dataLoader.LoadAsync(loader, StringHashHelper.ToStableHash(blobRef.Value.Rows[i].DataKey.ToString())); // [HEAP] BlobString.ToString()
            tasks[rowCount]     = _dataLoader.LoadAsync(loader, ResolveLocaleHash(languageCode));
            tasks[rowCount + 1] = _dataLoader.LoadAsync(loader, ResolveErrorLocaleHash(languageCode));

            await UniTask.WhenAll(tasks); // [HEAP] WhenAll 내부 배열 참조
        }

        private static int ResolveLocaleHash(string languageCode) => languageCode switch
        {
            "Ko" => SheetKey.BootstrapLocale_KoHash,
            "Jp" => SheetKey.BootstrapLocale_JpHash,
            _    => SheetKey.BootstrapLocale_EnHash,
        };

        private static int ResolveErrorLocaleHash(string languageCode) => languageCode switch
        {
            "Ko" => SheetKey.ErrorMsgLocale_KoHash,
            "Jp" => SheetKey.ErrorMsgLocale_JpHash,
            _    => SheetKey.ErrorMsgLocale_EnHash,
        };
    }
}
