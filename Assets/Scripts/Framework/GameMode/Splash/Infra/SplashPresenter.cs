using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.GameMode.Splash.Definitions;
using Elder.Framework.GameMode.Splash.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.GameMode.Splash.Infra
{
    internal sealed class SplashPresenter : UIPresenterBase, ISplashPresenter
    {
        private const string SplashLabel = "Textures/Splash";

        private IAssetRegistry _registry;
        private IUISystem _uiSystem;
        private ISplashViewModel _viewModel;

        protected override void OnInjectDependency()
        {
            if (!TryGetSystem<IAssetRegistry>(out _registry))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IAssetRegistry)}");
            if (!TryGetSystem<IUISystem>(out _uiSystem))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IUISystem)}");
        }

        public void SetViewModel(ISplashViewModel viewModel) => _viewModel = viewModel;

        public async UniTask PrepareAsync()
        {
            // TODO: IDataProvider에서 SplashEntryTable 로드 후 SetEntries 교체 예정
            _viewModel.SetEntries(new[]
            {
                new SplashEntry("TempSplash_000", 0.5f),
                new SplashEntry("TempSplash_001", 0.5f),
                new SplashEntry("TempSplash_002", 0.5f),
            });

            await _registry.RegisterAsync<Sprite>(SplashLabel);
        }

        public UniTask OnViewShownAsync()
        {
            if (!_uiSystem.TryGetView<SplashView>(out var view)) return UniTask.CompletedTask;
            if (_viewModel.Entries.Length > 0)
                view.Refresh(_registry.Get<Sprite>(SplashLabel, _viewModel.Entries[0].Key));
            return UniTask.CompletedTask;
        }

        public async UniTask PlaySplashSequenceAsync()
        {
            if (!_uiSystem.TryGetView<SplashView>(out var view)) return;

            var entries = _viewModel.Entries;
            for (int i = 0; i < entries.Length; i++)
            {
                await view.PlayFadeInAsync();

                if (entries[i].Interval > 0f)
                    await UniTask.Delay((int)(entries[i].Interval * 1000));

                int next = i + 1;
                Sprite nextSprite = next < entries.Length ? _registry.Get<Sprite>(SplashLabel, entries[next].Key) : null;
                await view.PlayFadeOutAsync(nextSprite);
            }
        }

        public UniTask OnViewHiddenAsync() => UniTask.CompletedTask;

        public UniTask OnViewReleasedAsync()
        {
            _registry?.Unregister(SplashLabel);
            return UniTask.CompletedTask;
        }

        protected override void DisposeManagedResources()
        {
            _registry = null;
            _uiSystem = null;
        }
    }
}
