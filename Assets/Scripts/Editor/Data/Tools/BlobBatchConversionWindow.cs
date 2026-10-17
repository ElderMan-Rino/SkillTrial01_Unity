/*
#if UNITY_EDITOR
using Elder.SkillTrial.Editor.Crypto;
using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Elder.Editor.Data.Tools
{
    public sealed class BlobBatchConversionWindow : EditorWindow
    {
        private const string BlobExtension = ".blob.bytes";
        private const string SourceExtension = ".bytes";
        private const string BootstrapDataTableName = "BootstrapData";
        private const string KeyConfigFilter = "t:EditorEncryptionKeyConfig";
        private const string DefaultAddressablesGroup = "GameData";
        private const string LocalInputFolderMarker = "/Local/";
        private const string InputFolderKey = "BlobBatch_InputFolder";
        private const string OutputFolderKey = "BlobBatch_OutputFolder";
        private const string LocalOutputFolderKey = "BlobBatch_LocalOutputFolder";
        private const string GroupNameKey = "BlobBatch_GroupName";
        private const string LocalGroupNameKey = "BlobBatch_LocalGroupName";

        private string _inputFolder = "Assets/Data/Source";
        private string _outputFolder = "Assets/Data/GeneratedBlob";
        private string _localOutputFolder = "Assets/AddressableAssets/Local/GameData";
        private string _addressablesGroup = DefaultAddressablesGroup;
        private string _localAddressablesGroup = DefaultAddressablesGroup;
        private Vector2 _scrollPos;
        private string _lastLog = string.Empty;
        private string _cachedLocalTablesInputFolder;
        private HashSet<string> _cachedLocalTableNames = new(StringComparer.Ordinal);

        [MenuItem("Elder/Data/Blob Batch Conversion Window")]
        public static void Open()
        {
            GetWindow<BlobBatchConversionWindow>("Blob Batch Conversion").Show();
        }

        private void OnEnable()
        {
            _inputFolder = EditorPrefs.GetString(InputFolderKey, _inputFolder);
            _outputFolder = EditorPrefs.GetString(OutputFolderKey, _outputFolder);
            _localOutputFolder = EditorPrefs.GetString(LocalOutputFolderKey, _localOutputFolder);
            _addressablesGroup = EditorPrefs.GetString(GroupNameKey, _addressablesGroup);
            _localAddressablesGroup = EditorPrefs.GetString(LocalGroupNameKey, _localAddressablesGroup);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Blob Batch Conversion", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            DrawFolderField("Input Folder (.bytes)", ref _inputFolder, InputFolderKey);
            DrawFolderField("Output Folder (.blob.bytes)", ref _outputFolder, OutputFolderKey);
            DrawFolderField("Local Output Folder (.blob.bytes)", ref _localOutputFolder, LocalOutputFolderKey);

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Addressables Group", EditorStyles.boldLabel);
            string newGroup = EditorGUILayout.TextField(_addressablesGroup);
            if (newGroup != _addressablesGroup)
            {
                _addressablesGroup = newGroup;
                EditorPrefs.SetString(GroupNameKey, _addressablesGroup);
            }

            EditorGUILayout.LabelField("Local Addressables Group", EditorStyles.boldLabel);
            string newLocalGroup = EditorGUILayout.TextField(_localAddressablesGroup);
            if (newLocalGroup != _localAddressablesGroup)
            {
                _localAddressablesGroup = newLocalGroup;
                EditorPrefs.SetString(LocalGroupNameKey, _localAddressablesGroup);
            }

            EditorGUILayout.Space(6);
            var localTableNames = ReadBootstrapLocalTableNames();
            EditorGUILayout.LabelField($"Local Tables (from {BootstrapDataTableName}): {localTableNames.Count}", EditorStyles.boldLabel);
            if (localTableNames.Count > 0)
                EditorGUILayout.HelpBox(string.Join(", ", localTableNames), MessageType.None);
            else
                EditorGUILayout.HelpBox($"{BootstrapDataTableName}{SourceExtension} 를 {_inputFolder} 에서 찾을 수 없습니다.", MessageType.Warning);

            EditorGUILayout.Space(10);

            bool canRun = Directory.Exists(_inputFolder) && !string.IsNullOrWhiteSpace(_outputFolder);
            GUI.enabled = canRun;
            if (GUILayout.Button("Convert All & Register Addressables", GUILayout.Height(36)))
                RunBatchConversion();
            GUI.enabled = true;

            if (!string.IsNullOrEmpty(_lastLog))
            {
                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField("Result Log", EditorStyles.boldLabel);
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(120));
                EditorGUILayout.SelectableLabel(_lastLog, EditorStyles.textArea,
                    GUILayout.Height(Mathf.Max(120, EditorStyles.textArea.lineHeight * (_lastLog.Split('\n').Length + 1))),
                    GUILayout.ExpandWidth(true));
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawFolderField(string label, ref string folderPath, string prefsKey)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            string newPath = EditorGUILayout.TextField(folderPath);
            if (newPath != folderPath)
            {
                folderPath = newPath;
                EditorPrefs.SetString(prefsKey, folderPath);
            }

            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string selected = EditorUtility.OpenFolderPanel(label, folderPath, string.Empty);
                if (!string.IsNullOrEmpty(selected))
                {
                    // 절대 경로 → Assets 상대 경로 변환
                    string dataPath = Application.dataPath;
                    if (selected.StartsWith(dataPath))
                        selected = "Assets" + selected.Substring(dataPath.Length);

                    folderPath = selected;
                    EditorPrefs.SetString(prefsKey, folderPath);
                    GUI.FocusControl(null);
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RunBatchConversion()
        {
            var keyConfig = LoadKeyConfig();
            if (keyConfig is null) return;

            byte[] keyPartB = Encoding.UTF8.GetBytes(keyConfig.KeyPartB);

            if (!Directory.Exists(_outputFolder))
                Directory.CreateDirectory(_outputFolder);
            if (!Directory.Exists(_localOutputFolder))
                Directory.CreateDirectory(_localOutputFolder);

            var localTableNames = ReadBootstrapLocalTableNames();
            string[] srcFiles = Directory.GetFiles(_inputFolder, $"*{SourceExtension}", SearchOption.TopDirectoryOnly);

            int success = 0;
            int fail = 0;
            var log = new StringBuilder();

            foreach (string absPath in srcFiles)
            {
                string srcPath = absPath.Replace('\\', '/');

                if (srcPath.EndsWith(BlobExtension, StringComparison.OrdinalIgnoreCase))
                    continue;

                string tableName = Path.GetFileNameWithoutExtension(srcPath);
                string outputFile = tableName + BlobExtension;
                bool isLocal = tableName == BootstrapDataTableName || localTableNames.Contains(GetDataKeyPrefix(tableName));
                string targetFolder = isLocal ? _localOutputFolder : _outputFolder;
                string targetGroup = isLocal ? _localAddressablesGroup : _addressablesGroup;
                string savePath = $"{targetFolder}/{outputFile}";

                var (_, bakeMethod) = FindBakerMethod(tableName);
                if (bakeMethod is null)
                {
                    log.AppendLine($"[SKIP] Baker not found: {tableName}");
                    fail++;
                    continue;
                }

                try
                {
                    // AesEncryptionProvider 생성자가 keyPartB를 ZeroMemory로 소거하므로 매 파일마다 복사본 전달
                    byte[] keyPartBCopy = (byte[])keyPartB.Clone();
                    bakeMethod.Invoke(null, new object[] { srcPath, savePath, keyPartBCopy });
                    AssetDatabase.ImportAsset(savePath);

                    string address = ResolveAddress(tableName);
                    RegisterToAddressables(savePath, address, targetGroup);

                    log.AppendLine($"[OK]{(isLocal ? "[Local]" : string.Empty)} {tableName} → {savePath}  (address: {address})");
                    success++;
                }
                catch (Exception ex)
                {
                    log.AppendLine($"[FAIL] {tableName}: {ex.InnerException?.Message ?? ex.Message}");
                    fail++;
                }
            }

            AssetDatabase.Refresh();
            _lastLog = log.ToString();

            EditorUtility.DisplayDialog("Blob Batch Conversion",
                $"Success: {success}  /  Fail: {fail}\nOutput: {_outputFolder}\nLocal Output: {_localOutputFolder}", "OK");
        }

        private HashSet<string> ReadBootstrapLocalTableNames()
        {
            if (_cachedLocalTablesInputFolder == _inputFolder)
                return _cachedLocalTableNames;

            _cachedLocalTablesInputFolder = _inputFolder;
            _cachedLocalTableNames = LoadBootstrapLocalTableNames(_inputFolder);
            return _cachedLocalTableNames;
        }

        // "ErrorMsgLocale_Ko" → "ErrorMsgLocale" (언더바 앞부분이 실제 데이터 타입)
        private static string GetDataKeyPrefix(string tableName)
        {
            int underscoreIndex = tableName.IndexOf('_');
            return underscoreIndex < 0 ? tableName : tableName[..underscoreIndex];
        }

        private static HashSet<string> LoadBootstrapLocalTableNames(string inputFolder)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);

            string srcPath = $"{inputFolder}/{BootstrapDataTableName}{SourceExtension}";
            if (!File.Exists(srcPath))
                return names;

            try
            {
                byte[] raw = File.ReadAllBytes(srcPath);
                var options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
                var rows = MessagePackSerializer.Deserialize<List<object[]>>(raw, options);

                foreach (object[] row in rows)
                {
                    // BootstrapDataBaker 스키마: row[1] = DataKey
                    if (row.Length > 1 && row[1] is string dataKey && !string.IsNullOrEmpty(dataKey))
                        names.Add(dataKey);
                }
                return names;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BlobBatch] {BootstrapDataTableName} 읽기 실패: {ex.Message}");
                return names;
            }
        }

        private static string ResolveAddress(string tableName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var sheetKeyType = Array.Find(assembly.GetTypes(), t => t.Name == "SheetKey");
                if (sheetKeyType == null) continue;

                var field = sheetKeyType.GetField(tableName, BindingFlags.Public | BindingFlags.Static);
                if (field != null)
                    return (string)field.GetValue(null);

                Debug.LogWarning($"[BlobBatch] SheetKey.{tableName} not found. DataForge CodeGenerator를 먼저 실행하세요.");
                return tableName;
            }

            Debug.LogWarning("[BlobBatch] SheetKey 클래스를 찾을 수 없습니다. DataForge CodeGenerator를 실행하세요.");
            return tableName;
        }

        private static (Type bakerType, MethodInfo bakeMethod) FindBakerMethod(string tableName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.Name.EndsWith("Baker")) continue;

                    if (type.Name == $"{tableName}Baker")
                    {
                        var m = type.GetMethod("Bake");
                        if (m is not null) return (type, m);
                    }

                    var subMethod = type.GetMethod($"Bake_{tableName}");
                    if (subMethod is not null) return (type, subMethod);
                }
            }
            return (null, null);
        }

        private static void RegisterToAddressables(string assetPath, string address, string groupName)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings is null)
            {
                Debug.LogWarning("[BlobBatch] AddressableAssetSettings not found.");
                return;
            }

            var group = settings.FindGroup(groupName)
                        ?? settings.CreateGroup(groupName, false, false, false, null);

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid)) return;

            var entry = settings.FindAssetEntry(guid) ?? settings.CreateOrMoveEntry(guid, group, false, false);
            entry.address = address;
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
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

            return AssetDatabase.LoadAssetAtPath<EditorEncryptionKeyConfig>(
                AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
#endif

*/
