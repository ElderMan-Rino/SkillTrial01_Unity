using Elder.Framework.Common.Utils;
using NUnit.Framework;

namespace Elder.Framework.Tests.EditMode.Framework.Common
{
    public class ManagedObjectPoolTests
    {
        private sealed class PooledItem
        {
            public bool IsRented;
        }

        [Test]
        public void Constructor_PreWarm_PopulatesIdleCount()
        {
            var pool = new ManagedObjectPool<PooledItem>(() => new PooledItem(), preWarm: 3);

            Assert.AreEqual(3, pool.IdleCount);
        }

        [Test]
        public void Rent_PoolHit_ReturnsFromStackAndDecrementsIdleCount()
        {
            var seed = new PooledItem();
            var pool = new ManagedObjectPool<PooledItem>(() => seed, preWarm: 1);

            var rented = pool.Rent();

            Assert.AreSame(seed, rented);
            Assert.AreEqual(0, pool.IdleCount);
        }

        [Test]
        public void Rent_PoolMiss_InvokesFactory()
        {
            var factoryCallCount = 0;
            var pool = new ManagedObjectPool<PooledItem>(() =>
            {
                factoryCallCount++;
                return new PooledItem();
            });

            pool.Rent();

            Assert.AreEqual(1, factoryCallCount);
        }

        [Test]
        public void Rent_InvokesOnRentCallback()
        {
            var pool = new ManagedObjectPool<PooledItem>(() => new PooledItem(), item => item.IsRented = true);

            var rented = pool.Rent();

            Assert.IsTrue(rented.IsRented);
        }

        [Test]
        public void Return_NullObject_DoesNotThrowOrChangeIdleCount()
        {
            var pool = new ManagedObjectPool<PooledItem>(() => new PooledItem());

            pool.Return(null);

            Assert.AreEqual(0, pool.IdleCount);
        }

        [Test]
        public void Return_ValidObject_IncrementsIdleCount()
        {
            var pool = new ManagedObjectPool<PooledItem>(() => new PooledItem());
            var rented = pool.Rent();

            pool.Return(rented);

            Assert.AreEqual(1, pool.IdleCount);
        }

        [Test]
        public void Return_InvokesOnReturnCallback()
        {
            var returnedItem = default(PooledItem);
            var pool = new ManagedObjectPool<PooledItem>(() => new PooledItem(), onReturn: item => returnedItem = item);
            var rented = pool.Rent();

            pool.Return(rented);

            Assert.AreSame(rented, returnedItem);
        }

        [Test]
        public void RentThenReturn_RoundTrip_RestoresOriginalIdleCount()
        {
            var pool = new ManagedObjectPool<PooledItem>(() => new PooledItem(), preWarm: 2);

            var rented = pool.Rent();
            pool.Return(rented);

            Assert.AreEqual(2, pool.IdleCount);
        }
    }
}
