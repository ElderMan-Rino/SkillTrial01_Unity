using Elder.Framework.Data.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.MainFrame.Infra.Configs;
using Elder.Framework.MainFrame.Installer;
using Elder.SkillTrial.Data.Installer;
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

            new GameInstaller().Install(builder);
            new FrameworkInstaller().Install(builder);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LogFacade.CleanUp();
        }
    }
}