using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Elder.Editor.Data.Tools
{
    public class MessageToBlobConverter : UnityEditor.Editor
    {
        private const string BlobExtension = ".blob.bytes"; 
        private const string SourceExtension = ".bytes";

        [MenuItem("Assets/Convert to DOTS Blob", false, 1)]
        public static void BakeSelectedBytesFiles()
        {
            var selectedObjects = Selection.objects;
            List<string> targetPaths = new List<string>();

            foreach (var obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (path.EndsWith(SourceExtension, StringComparison.OrdinalIgnoreCase) &&
                    !path.EndsWith(BlobExtension, StringComparison.OrdinalIgnoreCase))
                {
                    targetPaths.Add(path);
                }
            }

            if (targetPaths.Count == 0)
            {
                EditorUtility.DisplayDialog("DataBaking", "변환할 대상(.bytes)이 없거나 이미 변환된 파일입니다.", "확인");
                return;
            }

            int successCount = 0;
            int failCount = 0;

            foreach (string assetPath in targetPaths)
            {
                string tableName = Path.GetFileNameWithoutExtension(assetPath);

                string savePath = assetPath.Replace(SourceExtension, BlobExtension);
                Type bakerType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assm => assm.GetTypes())
                    .FirstOrDefault(t => t.Name == $"{tableName}Baker");

                if (bakerType != null)
                {
                    try
                    {
                        var bakeMethod = bakerType.GetMethod("Bake");
                        if (bakeMethod != null)
                        {
                            bakeMethod.Invoke(null, new object[] { assetPath, savePath });

                            AssetDatabase.ImportAsset(savePath);
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[DataBaking] {tableName} 변환 실패: {ex.InnerException?.Message ?? ex.Message}");
                        failCount++;
                    }
                }
                else
                {
                    Debug.LogWarning($"[DataBaking] {tableName}Baker를 찾을 수 없습니다.");
                    failCount++;
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("DataBaking 완료",
                $"총 {targetPaths.Count}개 중 {successCount}개 변환 성공.\n결과물: {BlobExtension}", "확인");
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
    }
}