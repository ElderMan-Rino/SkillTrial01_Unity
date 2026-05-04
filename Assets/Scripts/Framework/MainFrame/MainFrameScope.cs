using Elder.Framework.Data.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.MainFrame.Infra.Configs;
using Elder.Framework.MainFrame.Installer;
using Elder.SkillTrial.Resources;
using Elder.SkillTrial.Resources.Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.MainFrame
{
    public class MainFrameScope : LifetimeScope
    {
        [SerializeField] private FrameworkSettings _frameworkSettings;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<FrameworkSettings>(_frameworkSettings).As<IDataConfig>();
            builder.Register<GeneratedBlobLoader>(Lifetime.Singleton).As<IGameDataLoader>();
            new FrameworkInstaller().Install(builder);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LogFacade.CleanUp();
        }
    }
}