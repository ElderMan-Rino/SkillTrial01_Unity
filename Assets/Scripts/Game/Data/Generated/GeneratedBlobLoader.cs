using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.SkillTrial.Resources.Data
{
	public sealed class GeneratedBlobLoader : IGameDataLoader
	{
		private static readonly Dictionary<string, Func<IDataSheetLoader, UniTask>> _registry = new()
		{
			[SheetKey.SceneInfo]       = static l => l.LoadSheetAsync<SceneInfoRoot>(SheetKey.SceneInfo),
			[SheetKey.ErrorCode]       = static l => l.LoadSheetAsync<ErrorCodeRoot>(SheetKey.ErrorCode),
			[SheetKey.BootstrapData]   = static l => l.LoadSheetAsync<BootstrapDataRoot>(SheetKey.BootstrapData),
			[SheetKey.AudioSettings]   = static l => l.LoadSheetAsync<AudioSettingsRoot>(SheetKey.AudioSettings),
			[SheetKey.GraphicsSettings] = static l => l.LoadSheetAsync<GraphicsSettingsRoot>(SheetKey.GraphicsSettings),
			[SheetKey.LocaleSettings]  = static l => l.LoadSheetAsync<LocaleSettingsRoot>(SheetKey.LocaleSettings),
			["BootstrapLocale_Ko"]     = static l => l.LoadSheetAsync<LocaleEntryRoot>("BootstrapLocale_Ko"),
			["BootstrapLocale_En"]     = static l => l.LoadSheetAsync<LocaleEntryRoot>("BootstrapLocale_En"),
			["BootstrapLocale_Jp"]     = static l => l.LoadSheetAsync<LocaleEntryRoot>("BootstrapLocale_Jp"),
			["ErrorMsgLocale_Ko"]      = static l => l.LoadSheetAsync<LocaleEntryRoot>("ErrorMsgLocale_Ko"),
			["ErrorMsgLocale_En"]      = static l => l.LoadSheetAsync<LocaleEntryRoot>("ErrorMsgLocale_En"),
			["ErrorMsgLocale_Jp"]      = static l => l.LoadSheetAsync<LocaleEntryRoot>("ErrorMsgLocale_Jp"),
		};

		public UniTask LoadAsync<T>(IDataSheetLoader sheetLoader, string key) where T : unmanaged
			=> sheetLoader.LoadSheetAsync<T>(key);

		public UniTask LoadByKeyAsync(IDataSheetLoader sheetLoader, string key)
			=> _registry.TryGetValue(key, out var load)
				? load(sheetLoader)
				: throw new KeyNotFoundException(key);

		public async UniTask LoadAllAsync(IDataSheetLoader loader, IDataProvider provider, string languageCode)
		{
			await loader.LoadSheetAsync<BootstrapDataRoot>(SheetKey.BootstrapData);

			var handle = provider.GetData<BootstrapDataRoot>();
			if (handle is null || !handle.IsCreated) return;

			// IBlobDataHandle.GetRef() avoids copying BlobArray pointers (TryGetData copies struct which invalidates BlobArray offsets)
			if (handle is not IBlobDataHandle<BootstrapDataRoot> blobHandle) return;
			ref var root = ref blobHandle.GetRef();

			// [AOT RISK] generic instantiation per row count — bounded to small dataset
			var count = root.Rows.Length;
			for (int i = 0; i < count; i++)
			{
				var dataKey = root.Rows[i].DataKey.ToString();  // [HEAP] BlobString → string
				var resolvedKey = dataKey.Contains("{0}") ? dataKey.Replace("{0}", languageCode) : dataKey;  // [HEAP] Replace
				if (_registry.TryGetValue(resolvedKey, out var load))
					await load(loader);
			}
		}
	}
}
