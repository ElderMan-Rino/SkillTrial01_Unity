#if UNITY_EDITOR
using System;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace Elder.SkillTrial.Editor.Crypto
{
    public static class KeyPartBGenerator
    {
        private const string KeyConfigFilter = "t:EditorEncryptionKeyConfig";
        private const int KeyBytes = 32;

        [MenuItem("Elder/Crypto/Generate KeyPartB", false, 100)]
        public static void GenerateKeyPartB()
        {
            var keyConfig = LoadOrCreateKeyConfig();
            if (keyConfig == null) return;

            // [HEAP] 에디터 전용, 성능 무관
            byte[] randomBytes = new byte[KeyBytes];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(randomBytes);

            string keyPartB = Convert.ToBase64String(randomBytes);
            CryptographicOperations.ZeroMemory(randomBytes);

            keyConfig.SetKeyPartB(keyPartB);
            EditorUtility.SetDirty(keyConfig);
            AssetDatabase.SaveAssets();

            GUIUtility.systemCopyBuffer = keyPartB;

            EditorUtility.DisplayDialog(
                "KeyPartB 생성 완료",
                $"KeyPartB가 생성되어 EditorEncryptionKeyConfig에 저장되었습니다.\n\n" +
                $"클립보드에 복사되었습니다. 아래 값을 팀 노션 등 보안 채널에 공유하고,\n" +
                $"FrameworkSettings.EncryptionKeyPartB에도 동일하게 입력하세요.\n\n" +
                $"{keyPartB}",
                "확인");
        }

        private static EditorEncryptionKeyConfig LoadOrCreateKeyConfig()
        {
            string[] guids = AssetDatabase.FindAssets(KeyConfigFilter);

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<EditorEncryptionKeyConfig>(path);
            }

            bool create = EditorUtility.DisplayDialog(
                "EditorEncryptionKeyConfig 없음",
                "EditorEncryptionKeyConfig asset이 없습니다. 지금 생성하시겠습니까?\n" +
                "(Assets/Settings/ 폴더에 생성됩니다)",
                "생성", "취소");

            if (!create) return null;

            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                AssetDatabase.CreateFolder("Assets", "Settings");

            var config = ScriptableObject.CreateInstance<EditorEncryptionKeyConfig>();
            string savePath = "Assets/Settings/EditorEncryptionKeyConfig.asset";
            AssetDatabase.CreateAsset(config, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log($"[Crypto] EditorEncryptionKeyConfig 생성됨: {savePath}\n.gitignore에 추가하는 것을 권장합니다.");
            return config;
        }
    }
}
#endif
