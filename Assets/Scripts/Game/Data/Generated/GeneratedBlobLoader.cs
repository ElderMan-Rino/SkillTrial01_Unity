using Cysharp.Threading.Tasks;
using Elder.Framework.Data.Interfaces;
using Unity.Entities;

namespace Elder.SkillTrial.Resources.Data
{
	public sealed class GeneratedBlobLoader : IGameDataLoader
	{
		public async UniTask LoadAllAsync(IDataSheetLoader sheetLoader)
		{
			await UniTask.WhenAll(
				sheetLoader.LoadSheetAsync<SceneInfoRoot>("SceneInfo"),
				sheetLoader.LoadSheetAsync<LocaleEntryRoot>("LocaleEntry"),
				sheetLoader.LoadSheetAsync<ErrorCodeRoot>("ErrorCode")
			);
		}

		public async UniTask LoadAsync<T>(IDataSheetLoader sheetLoader, string key) where T : unmanaged
		{
			await sheetLoader.LoadSheetAsync<T>(key);
		}
	}
}
