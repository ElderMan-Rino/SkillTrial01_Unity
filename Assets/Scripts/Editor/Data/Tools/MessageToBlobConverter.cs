#if UNITY_EDITOR
using Elder.SkillTrial.Editor.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
                EditorUtility.DisplayDialog("DataBaking",
                    "변환할 파일(.bytes)이 없거나 이미 변환된 파일입니다.", "확인");
                return;
            }

            int successCount = 0;
            int failCount = 0;

            foreach (string assetPath in targetPaths)
            {
                string tableName = Path.GetFileNameWithoutExtension(assetPath);
                string savePath = assetPath.Replace(SourceExtension, BlobExtension);

                // ─── 기존 직접 탐색 제거, FindBakerMethod로 통합 ──────────
                var (bakerType, bakeMethod) = FindBakerMethod(tableName);

                if (bakerType is null || bakeMethod is null)
                {
                    Debug.LogWarning($"[DataBaking] {tableName} 에 대응하는 Baker를 찾을 수 없습니다.");
                    failCount++;
                    continue;
                }

                try
                {
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

        // ─── Baker 탐색 (1순위: 기존 / 2순위: 언어 공통 Baker) ────────────
        private static (Type bakerType, MethodInfo bakeMethod) FindBakerMethod(
            string tableName)
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes());

            // 1순위: "{TableName}Baker.Bake()" — 기존 일반 테이블
            var directBaker = allTypes
                .FirstOrDefault(t => t.Name == $"{tableName}Baker");
            if (directBaker != null)
            {
                var method = directBaker.GetMethod("Bake");
                if (method != null) return (directBaker, method);
            }

            // 2순위: "{DataName}Baker.Bake_{TableName}()" — 언어 공통 Baker
            // "UI_Ko" → "Bake_UI_Ko" 메서드를 가진 Baker 클래스 탐색
            string targetMethodName = $"Bake_{tableName}";
            foreach (var type in allTypes)
            {
                if (!type.Name.EndsWith("Baker")) continue;
                var method = type.GetMethod(targetMethodName);
                if (method != null) return (type, method);
            }

            return (null, null);
        }

        private static EditorEncryptionKeyConfig LoadKeyConfig()
        {
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