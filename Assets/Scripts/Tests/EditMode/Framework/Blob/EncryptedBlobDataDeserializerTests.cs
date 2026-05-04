using Elder.Framework.Blob.Infra;
using Elder.Framework.Crypto.Interfaces;
using Elder.Framework.Data.Interfaces;
using NUnit.Framework;
using System;

namespace Elder.Framework.Tests.Blob
{
    // EncryptedBlobDataDeserializer.Deserialize delegates to BlobDataDeserializer,
    // which requires Unity.Entities (ECS runtime). Integration tests covering the
    // full decrypt→deserialize path belong in PlayMode. These EditMode tests cover
    // the guard clause and the encryption provider interaction that precede the ECS call.
    internal sealed class EncryptedBlobDataDeserializerTests
    {
        // ─── Guard clause ──────────────────────────────────────────────────────

        [Test]
        public void Deserialize_WhenDataIsNull_ThrowsArgumentException()
        {
            var deserializer = new EncryptedBlobDataDeserializer(
                new SpyEncryptionProvider(), inner: null);

            Assert.Throws<ArgumentException>(() => deserializer.Deserialize<DummyBlob>(null));
        }

        [Test]
        public void Deserialize_WhenDataIsEmpty_ThrowsArgumentException()
        {
            var deserializer = new EncryptedBlobDataDeserializer(
                new SpyEncryptionProvider(), inner: null);

            Assert.Throws<ArgumentException>(() => deserializer.Deserialize<DummyBlob>(Array.Empty<byte>()));
        }

        // ─── Encryption provider interaction ──────────────────────────────────

        [Test]
        public void Deserialize_CallsGetDecryptedMaxSizeWithDataLength()
        {
            var spy = new SpyEncryptionProvider { DecryptedMaxSize = 0 };
            var deserializer = new EncryptedBlobDataDeserializer(spy, inner: null);
            byte[] data = { 0x01, 0x02 };

            // Will throw after GetDecryptedMaxSize because inner is null —
            // we only verify GetDecryptedMaxSize was called with the correct length.
            try { deserializer.Deserialize<DummyBlob>(data); } catch { }

            Assert.AreEqual(data.Length, spy.LastGetDecryptedMaxSizeArg);
        }

        [Test]
        public void Deserialize_CallsDecryptWithFullDataSpan()
        {
            var spy = new SpyEncryptionProvider { DecryptedMaxSize = 32 };
            var deserializer = new EncryptedBlobDataDeserializer(spy, inner: null);
            byte[] data = { 0x01, 0x02, 0x03 };

            try { deserializer.Deserialize<DummyBlob>(data); } catch { }

            Assert.IsTrue(spy.DecryptCalled);
            Assert.AreEqual(data.Length, spy.LastDecryptCiphertextLength);
        }

        // ─── Fixtures ──────────────────────────────────────────────────────────

        private struct DummyBlob { }

        private sealed class SpyEncryptionProvider : IEncryptionProvider
        {
            public int DecryptedMaxSize;
            public bool DecryptCalled;
            public int LastGetDecryptedMaxSizeArg;
            public int LastDecryptCiphertextLength;

            public int GetDecryptedMaxSize(int ciphertextLength)
            {
                LastGetDecryptedMaxSizeArg = ciphertextLength;
                return DecryptedMaxSize;
            }

            public int Decrypt(ReadOnlySpan<byte> ciphertext, Span<byte> destination)
            {
                DecryptCalled = true;
                LastDecryptCiphertextLength = ciphertext.Length;
                return 0;
            }

            public int GetEncryptedSize(int plaintextLength) => 0;
            public int Encrypt(ReadOnlySpan<byte> plaintext, Span<byte> destination) => 0;
            public void Dispose() { }
        }
    }
}
