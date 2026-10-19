using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Root.Infra;
using System;
using UnityEngine;

namespace Elder.TestModule.Common.Boot
{
    internal static class TestBootEntry
    {
        public static void Run(GameObject host, Action<IComponentRegistry> installAction, Type startCoordinatorType)
        {
            var anchor = host.AddComponent<MainFrameworkAnchor>();
            var root = new TestModuleRoot();
            anchor.BindRoot(root);
            root.BootModuleAsync(installAction, startCoordinatorType, host.GetCancellationTokenOnDestroy()).Forget();
        }
    }
}
