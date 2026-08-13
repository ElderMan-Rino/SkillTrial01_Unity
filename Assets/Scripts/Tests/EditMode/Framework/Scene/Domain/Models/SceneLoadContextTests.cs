using Elder.Framework.Scene.Domain.Models;
using NUnit.Framework;
using System.Linq;

namespace Elder.Framework.Tests.EditMode.Framework.Scene.Domain.Models
{
    public class SceneLoadContextTests
    {
        [Test]
        public void Initialize_SetsMainSceneName()
        {
            var context = new SceneLoadContext();

            context.Initialize("MainScene");

            Assert.AreEqual("MainScene", context.MainSceneName);
        }

        [Test]
        public void TryAddSubScene_NewName_ReturnsTrueAndStoresIt()
        {
            var context = new SceneLoadContext();

            bool added = context.TryAddSubScene("SubA");

            Assert.IsTrue(added);
            CollectionAssert.Contains(context.SubSceneNames.ToList(), "SubA");
        }

        [Test]
        public void TryAddSubScene_DuplicateName_ReturnsFalse()
        {
            var context = new SceneLoadContext();
            context.TryAddSubScene("SubA");

            bool added = context.TryAddSubScene("SubA");

            Assert.IsFalse(added);
            Assert.AreEqual(1, context.SubSceneNames.Count());
        }

        [Test]
        public void Dispose_ClearsMainSceneNameAndSubScenes()
        {
            var context = new SceneLoadContext();
            context.Initialize("MainScene");
            context.TryAddSubScene("SubA");

            context.Dispose();

            Assert.IsNull(context.MainSceneName);
            Assert.IsEmpty(context.SubSceneNames);
        }
    }
}
