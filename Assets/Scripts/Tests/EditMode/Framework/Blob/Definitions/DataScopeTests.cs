using Elder.Framework.Blob.Definitions;
using NUnit.Framework;

namespace Elder.Framework.Tests.EditMode.Framework.Blob.Definitions
{
    public class DataScopeTests
    {
        [Test]
        public void PersistentHash_MatchesStableHashOfName()
        {
            Assert.AreEqual(Elder.Framework.Common.Utils.StringHashHelper.ToStableHash(DataScope.Persistent), DataScope.PersistentHash);
        }

        [Test]
        public void BootHash_MatchesStableHashOfName()
        {
            Assert.AreEqual(Elder.Framework.Common.Utils.StringHashHelper.ToStableHash(DataScope.Boot), DataScope.BootHash);
        }

        [Test]
        public void PreloadHash_MatchesStableHashOfName()
        {
            Assert.AreEqual(Elder.Framework.Common.Utils.StringHashHelper.ToStableHash(DataScope.Preload), DataScope.PreloadHash);
        }

        [Test]
        public void ScopeHashes_AreAllDistinct()
        {
            Assert.AreNotEqual(DataScope.PersistentHash, DataScope.BootHash);
            Assert.AreNotEqual(DataScope.BootHash, DataScope.PreloadHash);
            Assert.AreNotEqual(DataScope.PersistentHash, DataScope.PreloadHash);
        }
    }
}
