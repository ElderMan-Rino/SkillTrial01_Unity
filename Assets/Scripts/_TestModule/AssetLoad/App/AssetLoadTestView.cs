using Elder.Framework.Core.UI.App;
using Elder.Framework.Log.Definitions.Enums;
using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Elder.TestModule.AssetLoad.App
{
    // AssetModuleTest 씬 전용 더미 View. Addressable 생성 경로와 씬 바인딩 경로 검증용.
    internal sealed class AssetLoadTestView : UIViewBase
    {
        [SerializeField] private PanelRenderer _panelRenderer;

        private readonly ReactiveCommand<Unit> _loadAssetClick = new(); // [HEAP]
        private readonly ReactiveCommand<Unit> _listCacheClick = new(); // [HEAP]
        private readonly ReactiveCommand<Unit> _releaseAllClick = new(); // [HEAP]
        private readonly ReactiveCommand<Unit> _memoryStatusClick = new(); // [HEAP]
        private readonly ReactiveCommand<Unit> _clearLogClick = new(); // [HEAP]

        private Button _loadAssetButton;
        private Button _listCacheButton;
        private Button _releaseAllButton;
        private Button _memoryStatusButton;
        private Button _clearLogButton;
        private ScrollView _logScrollView;
        private VisualElement _logContainer;
        private ObjectPool<Label> _logLabelPool;
        private readonly List<Label> _activeLogLabels = new(); // [HEAP]

        public Observable<Unit> OnLoadAssetRequested => _loadAssetClick;
        public Observable<Unit> OnListCacheRequested => _listCacheClick;
        public Observable<Unit> OnReleaseAllRequested => _releaseAllClick;
        public Observable<Unit> OnMemoryStatusRequested => _memoryStatusClick;
        public Observable<Unit> OnClearLogRequested => _clearLogClick;

        private void Awake()
        {
            _logLabelPool = new ObjectPool<Label>(CreateLogLabel, actionOnRelease: OnLogLabelRelease); // [HEAP]
            _panelRenderer.RegisterUIReloadCallback(HandlePanelReloaded);
        }

        private void OnDestroy()
        {
            _panelRenderer.UnregisterUIReloadCallback(HandlePanelReloaded);
            if (_loadAssetButton is not null)
            {
                UnsubscribeButtonEvents();
            }

            _loadAssetClick.Dispose();
            _listCacheClick.Dispose();
            _releaseAllClick.Dispose();
            _memoryStatusClick.Dispose();
            _clearLogClick.Dispose();
            _logLabelPool.Dispose();
        }

        public void AppendLog(LogLevel level, string message)
        {
            var label = _logLabelPool.Get();
            label.text = message;
            label.AddToClassList(GetLevelClassName(level));
            _logContainer.Add(label);
            _activeLogLabels.Add(label);
            _logScrollView.scrollOffset = new Vector2(0f, float.MaxValue);
        }

        public void ClearLog()
        {
            foreach (var label in _activeLogLabels)
            {
                _logLabelPool.Release(label);
            }
            _activeLogLabels.Clear();
            _logContainer.Clear();
        }

        public void SetLoadAssetButtonBusy(bool isBusy)
        {
            _loadAssetButton.SetEnabled(!isBusy);
            _loadAssetButton.text = isBusy ? "Loading..." : "Load Asset";
        }

        private void HandlePanelReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            if (_loadAssetButton is not null)
            {
                UnsubscribeButtonEvents();
            }

            SetUIElements(rootElement);
            SubscribeButtonEvents();
        }

        private void SetUIElements(VisualElement root)
        {
            _loadAssetButton = root.Q<Button>("load-asset-button");
            _listCacheButton = root.Q<Button>("list-cache-button");
            _releaseAllButton = root.Q<Button>("release-all-button");
            _memoryStatusButton = root.Q<Button>("memory-status-button");
            _clearLogButton = root.Q<Button>("clear-log-button");
            _logScrollView = root.Q<ScrollView>("log-scroll-view");
            _logContainer = root.Q<VisualElement>("log-container");
        }

        private void SubscribeButtonEvents()
        {
            _loadAssetButton.clicked += OnLoadAssetClicked;
            _listCacheButton.clicked += OnListCacheClicked;
            _releaseAllButton.clicked += OnReleaseAllClicked;
            _memoryStatusButton.clicked += OnMemoryStatusClicked;
            _clearLogButton.clicked += OnClearLogClicked;
        }

        private void UnsubscribeButtonEvents()
        {
            _loadAssetButton.clicked -= OnLoadAssetClicked;
            _listCacheButton.clicked -= OnListCacheClicked;
            _releaseAllButton.clicked -= OnReleaseAllClicked;
            _memoryStatusButton.clicked -= OnMemoryStatusClicked;
            _clearLogButton.clicked -= OnClearLogClicked;
        }

        private void OnLoadAssetClicked()
        {
            _loadAssetClick.Execute(Unit.Default);
        }

        private void OnReleaseAllClicked()
        {
            _releaseAllClick.Execute(Unit.Default);
        }

        private void OnListCacheClicked()
        {
            _listCacheClick.Execute(Unit.Default);
        }

        private void OnMemoryStatusClicked()
        {
            _memoryStatusClick.Execute(Unit.Default);
        }

        private void OnClearLogClicked()
        {
            _clearLogClick.Execute(Unit.Default);
        }

        private static Label CreateLogLabel()
        {
            var label = new Label(); // [HEAP]
            label.AddToClassList("log-line");
            return label;
        }

        private static void OnLogLabelRelease(Label label)
        {
            label.RemoveFromHierarchy();
            label.ClearClassList();
            label.AddToClassList("log-line");
        }

        private static string GetLevelClassName(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => "log-line-debug",
                LogLevel.Info => "log-line-info",
                LogLevel.Warning => "log-line-warning",
                LogLevel.Error => "log-line-error",
                _ => "log-line-info"
            };
        }
    }
}
