using Elder.Framework.Scene.Infra;
using Elder.Framework.Scene.Interfaces;
using NUnit.Framework;
using System;

namespace Elder.Framework.Tests.EditMode.Framework.Scene.Infra
{
    public class SceneContextFactoryTests
    {
        private sealed class FakeSceneLoadContext : ISceneLoadContext
        {
            public string MainSceneName => null;
            public System.Collections.Generic.IEnumerable<string> SubSceneNames => Array.Empty<string>();
            public bool TryAddSubScene(string subSceneName) => false;
        }

        [Test]
        public void Create_ReturnsContextWithGivenMainSceneName()
        {
            var factory = new SceneContextFactory();

            var context = factory.Create("MainScene");

            Assert.AreEqual("MainScene", context.MainSceneName);
        }

        [Test]
        public void Release_ThenCreate_ReusesPooledContext()
        {
            var factory = new SceneContextFactory();
            var first = factory.Create("MainScene");
            factory.Release(first);

            var second = factory.Create("OtherScene");

            Assert.AreSame(first, second);
            Assert.AreEqual("OtherScene", second.MainSceneName);
        }

        [Test]
        public void Release_UnexpectedContextType_ThrowsArgumentException()
        {
            var factory = new SceneContextFactory();
            var fake = new FakeSceneLoadContext();

            Assert.Throws<ArgumentException>(() => factory.Release(fake));
        }
    }
}
