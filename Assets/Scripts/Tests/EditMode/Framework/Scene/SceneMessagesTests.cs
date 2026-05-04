using Elder.Framework.Scene.Messages;
using NUnit.Framework;

namespace Elder.Framework.Tests.Scene
{
    internal sealed class SceneMessagesTests
    {
        [Test]
        public void FxSceneTransition_StoresTargetKey()
        {
            var msg = new FxSceneTransition("GameScene");
            Assert.AreEqual("GameScene", msg.TargetSceneKey);
        }

        [Test]
        public void FxSceneTransitionStarted_StoresTargetKey()
        {
            var msg = new FxSceneTransitionStarted("GameScene");
            Assert.AreEqual("GameScene", msg.TargetSceneKey);
        }

        [Test]
        public void FxSceneTransitionCompleted_StoresLoadedKey()
        {
            var msg = new FxSceneTransitionCompleted("GameScene");
            Assert.AreEqual("GameScene", msg.LoadedSceneKey);
        }

        [Test]
        public void FxSceneTransition_IsReadonlyStruct_DefaultKeyIsNull()
        {
            var msg = default(FxSceneTransition);
            Assert.IsNull(msg.TargetSceneKey);
        }
    }
}
