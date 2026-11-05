using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Elder.SkillTrial.Editor.Build
{
	public static class AddressableCliBuilder
	{
		public static void BuildAddressables()
		{
			var settings = AddressableAssetSettingsDefaultObject.Settings;
			if (settings is null)
			{
				Debug.LogError("AddressableAssetSettings not found.");
				EditorApplication.Exit(1);
				return;
			}

			AddressableAssetSettings.BuildPlayerContent(out var result);

			if (!string.IsNullOrEmpty(result.Error))
			{
				Debug.LogError($"Addressable build failed: {result.Error}");
				EditorApplication.Exit(1);
				return;
			}

			Debug.Log($"Addressable build succeeded: {result.OutputPath}");
			EditorApplication.Exit(0);
		}

		public static void CleanAddressables()
		{
			AddressableAssetSettings.CleanPlayerContent();
			EditorApplication.Exit(0);
		}
	}
}
