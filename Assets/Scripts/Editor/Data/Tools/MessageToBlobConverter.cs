#if UNITY_EDITOR
using Elder.SkillTrial.Editor.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Elder.Editor.Data.Tools
{
    public class MessageToBlobConverter : UnityEditor.Editor
    {
        private const string BlobExtension = ".blob.bytes";
        private const string SourceExtension = ".bytes";
        private const string KeyConfigFilter = "t:EditorEncryptionKeyConfig";

        [MenuItem("Assets/Convert to DOTS Blob", false, 1)]
        public static void BakeSelectedBytesFiles()
        {
            var keyConfig = LoadKeyConfig();
            if (keyConfig == null) return;

            // [HEAP] UTF-8 변환 — 에디터 전용, 성능 무관
            byte[] keyPartB = Encoding.UTF8.GetBytes(keyConfig.KeyPartB);

            var selectedObjects = Selection.objects;
            var targetPaths = new List<string>();

            foreach (var obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (path.EndsWith(SourceExtension, StringComparison.OrdinalIgnoreCase) &&
                    !path.EndsWith(BlobExtension, StringComparison.OrdinalIgnoreCase))
                    targetPaths.Add(path);
            }

            if (targetPaths.Count == 0)
            {
                EditorUtility.DisplayDialog("DataBaking", "변환할 파일(.bytes)이 없거나 이미 변환된 파일입니다.", "확인");
                return;
            }

            int successCount = 0;
            int failCount = 0;

            foreach (string assetPath in targetPaths)
            {
                string tableName = Path.GetFileNameWithoutExtension(assetPath);
                string savePath = assetPath.Replace(SourceExtension, BlobExtension);

                // [AOT RISK] 리플렉션 — 에디터 전용, 런타임 미사용
                Type bakerType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == $"{tableName}Baker");

                if (bakerType is null)
                {
                    Debug.LogWarning($"[DataBaking] {tableName}Baker를 찾을 수 없습니다.");
                    failCount++;
                    continue;
                }

                try
                {
                    var bakeMethod = bakerType.GetMethod("Bake");
                    if (bakeMethod is null)
                    {
                        Debug.LogWarning($"[DataBaking] {tableName}Baker.Bake 메서드를 찾을 수 없습니다.");
                        failCount++;
                        continue;
                    }

                    bakeMethod.Invoke(null, new object[] { assetPath, savePath, keyPartB });
                    AssetDatabase.ImportAsset(savePath);
                    successCount++;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[DataBaking] {tableName} 변환 실패: {ex.InnerException?.Message ?? ex.Message}");
                    failCount++;
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("DataBaking 완료",
                $"총 {targetPaths.Count}개 중 {successCount}개 변환 성공.\n출력: {BlobExtension}", "확인");
        }

        [MenuItem("Assets/Convert to DOTS Blob", true)]
        private static bool ValidateBakeSelectedBytesFiles()
        {
            return Selection.objects.Any(obj =>
            {
                string path = AssetDatabase.GetAssetPath(obj);
                return path.EndsWith(SourceExtension, StringComparison.OrdinalIgnoreCase) &&
                       !path.EndsWith(BlobExtension, StringComparison.OrdinalIgnoreCase);
            });
        }

        private static EditorEncryptionKeyConfig LoadKeyConfig()
        {
            // [AOT RISK] FindAssets — 에디터 전용
            string[] guids = AssetDatabase.FindAssets(KeyConfigFilter);
            if (guids.Length == 0)
            {
                EditorUtility.DisplayDialog("암호화 키 없음",
                    "EditorEncryptionKeyConfig asset이 없습니다.\n" +
                    "메뉴 Elder/Crypto/Create Encryption Key Config로 생성 후 KeyPartB를 설정하세요.", "확인");
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<EditorEncryptionKeyConfig>(path);
        }
    }
}
#endif
