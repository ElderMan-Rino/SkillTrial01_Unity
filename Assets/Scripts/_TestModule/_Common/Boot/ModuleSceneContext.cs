using Elder.Framework.Core.Interfaces;
using System;
using UnityEngine;

namespace Elder.TestModule.Common.Boot
{
    internal abstract class ModuleSceneContext : MonoBehaviour
    {
        private void Awake()
        {
            TestBootEntry.Run(gameObject, GetInstallAction(), GetStartCoordinatorType());
        }

        protected abstract Action<IComponentRegistry> GetInstallAction();

        protected abstract Type GetStartCoordinatorType();
    }
}
