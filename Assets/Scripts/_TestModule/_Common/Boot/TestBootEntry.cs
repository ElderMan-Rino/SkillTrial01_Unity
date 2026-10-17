using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.Infra;
using Elder.Framework.Root.Infra;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Signal.Messages;
using System;
using UnityEngine;

namespace Elder.TestModule.Common.Boot
{
    internal static class TestBootEntry
    {
        public static void Run(GameObject host, Action<IComponentRegistry> installAction, Type startCoordinatorType)
        {
            var anchor = host.AddComponent<MainFrameworkAnchor>();
            var root = new TestModuleRoot(installAction);
            anchor.BindRoot(root);
            root.Initialize();

            if (root.TryGetComponent<ISignalPublisher>(out var publisher))
            {
                //host.AddComponent<UIInputBridge>().Initialize(publisher);
                publisher.Publish(new FxModuleStartSignal(startCoordinatorType));
            }
        }
    }
}
