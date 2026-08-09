using Elder.Framework.Common.Utils;
using NUnit.Framework;
using Unity.Collections;

namespace Elder.Framework.Tests.EditMode.Framework.Common
{
    public class NativeArrayPoolTests
    {
        [Test]
        public void Rent_PoolMiss_ReturnsArrayWithNextPow2Length()
        {
            var pool = new NativeArrayPool<int>();

            var array = pool.Rent(5);

            Assert.AreEqual(8, array.Length);
            array.Dispose();
        }

        [Test]
        public void Rent_ExactPow2Length_DoesNotRoundUp()
        {
            var pool = new NativeArrayPool<int>();

            var array = pool.Rent(8);

            Assert.AreEqual(8, array.Length);
            array.Dispose();
        }

        [Test]
        public void Rent_ZeroOrNegativeLength_ReturnsLengthOne()
        {
            var pool = new NativeArrayPool<int>();

            var array = pool.Rent(0);

            Assert.AreEqual(1, array.Length);
            array.Dispose();
        }

        [Test]
        public void Return_ThenRent_ReusesSameArray()
        {
            var pool = new NativeArrayPool<int>();
            var rented = pool.Rent(4);

            pool.Return(rented);
            var reused = pool.Rent(4);

            Assert.AreEqual(rented, reused);
            reused.Dispose();
        }

        [Test]
        public void Return_UncreatedArray_DoesNotThrow()
        {
            var pool = new NativeArrayPool<int>();

            Assert.DoesNotThrow(() => pool.Return(default));
        }

        [Test]
        public void Dispose_DisposesAllPooledArrays()
        {
            var pool = new NativeArrayPool<int>();
            var array = pool.Rent(4);
            pool.Return(array);

            pool.Dispose();

            Assert.IsFalse(array.IsCreated);
        }

        [Test]
        public void Dispose_CalledTwice_DoesNotThrow()
        {
            var pool = new NativeArrayPool<int>();
            pool.Dispose();

            Assert.DoesNotThrow(() => pool.Dispose());
        }
    }
}
