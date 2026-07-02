using System;
using System.Collections.Generic;
using Elder.Framework.Scene.Interfaces;

namespace Elder.Framework.Scene.Domain.Models
{
    internal sealed class SceneLoadContext : ISceneLoadContext, IDisposable
    {
        private readonly HashSet<string> _subSceneNames = new();

        public string MainSceneName { get; private set; }
        public IEnumerable<string> SubSceneNames => _subSceneNames;

        public SceneLoadContext Initialize(string mainSceneName)
        {
            MainSceneName = mainSceneName;
            return this;
        }

        public bool TryAddSubScene(string subSceneName)
        {
            return _subSceneNames.Add(subSceneName);
        }

        public void Dispose()
        {
            ClearSubSceneNames();
            ClearMainSceneName();
        }

        private void ClearMainSceneName()
        {
            MainSceneName = null;
        }

        private void ClearSubSceneNames()
        {
            _subSceneNames.Clear();
        }
    }
}