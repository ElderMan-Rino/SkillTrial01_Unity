using System.Collections.Generic;

namespace Elder.Framework.Scene.Interfaces
{
    public interface ISceneLoadContext
    {
        public string MainSceneName { get; }
        public IEnumerable<string> SubSceneNames { get; }
        public bool TryAddSubScene(string subSceneName);
    }
}
