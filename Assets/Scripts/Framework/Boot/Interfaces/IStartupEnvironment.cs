using Elder.Framework.Core.Interfaces;
using UnityEngine;

namespace Elder.Framework.Boot.Interfaces
{
    public interface IStartupEnvironment : IGameSystem
    {
        public bool IsInitSceneActive();
    }
}