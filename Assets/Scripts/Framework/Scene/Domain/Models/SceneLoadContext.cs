using System;
using System.Collections.Generic;

namespace Elder.Framework.Scene.Domain.Data
{
    public class SceneLoadContext : IDisposable
    {
        private readonly HashSet<string> _subSceneNames = new();

        public string MainSceneName { get; private set; }
        public IEnumerable<string> SubSceneNames => _subSceneNames;

        public SceneLoadContext Initialize(string mainScenName)
        {
            MainSceneName = mainScenName; 
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