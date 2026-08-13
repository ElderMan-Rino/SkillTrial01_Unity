using Elder.Framework.Scene.Messages;
using NUnit.Framework;

namespace Elder.Framework.Tests.EditMode.Framework.Scene.Messages
{
    public class SceneTransitionSignalsTests
    {
        [Test]
        public void SceneTransitionSignal_StoresTargetSceneKey()
        {
            var signal = new SceneTransitionSignal("MainScene");

            Assert.AreEqual("MainScene", signal.TargetSceneKey);
        }

        [Test]
        public void SceneTransitionStartedSignal_StoresTargetSceneKey()
        {
            var signal = new SceneTransitionStartedSignal("MainScene");

            Assert.AreEqual("MainScene", signal.TargetSceneKey);
        }

        [Test]
        public void SceneTransitionCompletedSignal_StoresLoadedSceneKey()
        {
            var signal = new SceneTransitionCompletedSignal("MainScene");

            Assert.AreEqual("MainScene", signal.LoadedSceneKey);
        }
    }
}
