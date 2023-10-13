using AnnOtter.WayToSecureExchange.Helpers;
using System.Security.Cryptography;

namespace AnnOtter.WayToSecureExchange.Tests.Helpers
{
    [TestClass]
    public class CryptoHelperTests
    {
        [TestMethod]
        public void GetSha256Hash_ValidInput_ReturnsExpectedHash()
        {
            // Arrange
            string inputString = "Hello, World!";

            // Act
            string hash = CryptoHelper.GetSha256Hash(inputString);

            // Assert
            Assert.AreEqual("dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f", hash);
        }

        [TestMethod]
        public void GetSha256Hash_EmptyInput_ReturnsDefinedHash()
        {
            // Arrange
            string inputString = string.Empty;

            // Act
            string hash = CryptoHelper.GetSha256Hash(inputString);

            // Assert
            // Test vectors: https://www.di-mgt.com.au/sha_testvectors.html
            Assert.AreEqual("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", hash);
        }

        [TestMethod]
        public void EncryptAndDecryptChaCha20Poly1305_ValidInput_RoundTrip()
        {
            // Arrange
            string plaintext = "This is a test message.";
            Span<byte> keyBytes = stackalloc byte[32];
            RandomNumberGenerator.Fill(keyBytes);

            string key = Convert.ToBase64String(keyBytes);

            // Act
            var encryptionResponse = CryptoHelper.EncryptChaCha20Poly1305(plaintext, key);
            string decryptedText = CryptoHelper.DecryptChaCha20Poly1305(encryptionResponse.Ciphertext, encryptionResponse.Nonce, key, encryptionResponse.Tag);

            // Assert
            Assert.AreEqual(plaintext, decryptedText);
        }

        [TestMethod]
        public void EncryptChaCha20Poly1305_InvalidInput_NullOrEmptyKey_ThrowsException()
        {
            // Arrange
            string plaintext = "This is a test message.";

            // Act and Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                CryptoHelper.EncryptChaCha20Poly1305(plaintext, null);
            });

            Assert.ThrowsException<CryptographicException>(() =>
            {
                CryptoHelper.EncryptChaCha20Poly1305(plaintext, string.Empty);
            });
        }

        [TestMethod]
        public void EncryptAndDecryptChaCha20Poly1305_InvalidTag_ThrowsException()
        {
            // Arrange
            string plaintext = "This is a test message.";
            Span<byte> keyBytes = stackalloc byte[32];
            RandomNumberGenerator.Fill(keyBytes);

            string key = Convert.ToBase64String(keyBytes);

            var encryptionResponse = CryptoHelper.EncryptChaCha20Poly1305(plaintext, key);

            // Modify the tag to simulate an invalid tag
            encryptionResponse.Tag = Convert.ToBase64String(stackalloc byte[16]);

            // Act and Assert
            Assert.ThrowsException<CryptographicException>(() =>
            {
                CryptoHelper.DecryptChaCha20Poly1305(encryptionResponse.Ciphertext, encryptionResponse.Nonce, key, encryptionResponse.Tag);
            });
        }

        [TestMethod]
        [Timeout(100)]
        public void EncryptAndDecryptChaCha20Poly1305_PerformanceTest()
        {
            // Arrange
            var plaintext = new string('A', 1000000); // 1 MB plaintext

            Span<byte> keyBytes = stackalloc byte[32];
            RandomNumberGenerator.Fill(keyBytes);

            string key = Convert.ToBase64String(keyBytes);

            // Act
            var encryptionResponse = CryptoHelper.EncryptChaCha20Poly1305(plaintext, key);
            string decryptedText = CryptoHelper.DecryptChaCha20Poly1305(encryptionResponse.Ciphertext, encryptionResponse.Nonce, key, encryptionResponse.Tag);

            // Assert
            Assert.AreEqual(plaintext, decryptedText);
        }

        [TestMethod]
        public void ConcurrentEncryptChaCha20Poly1305()
        {
            // Arrange
            Span<byte> keyBytes = stackalloc byte[32];
            RandomNumberGenerator.Fill(keyBytes);

            string key = Convert.ToBase64String(keyBytes);

            int numberOfThreads = 10;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var tasks = new Task[numberOfThreads];

            for (int i = 0; i < numberOfThreads; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    var random = new Random();
                    var randomString = new string(Enumerable.Repeat(chars, 100).Select(s => s[random.Next(s.Length)]).ToArray());
                    string plaintext = "This is a test message: " + randomString;

                    // Act
                    var encryptionResponse = CryptoHelper.EncryptChaCha20Poly1305(plaintext, key);
                    string decryptedText = CryptoHelper.DecryptChaCha20Poly1305(encryptionResponse.Ciphertext, encryptionResponse.Nonce, key, encryptionResponse.Tag);

                    // Assert
                    Assert.AreEqual(plaintext, decryptedText);
                });
            }

            // Wait for all tasks to complete
            Task.WaitAll(tasks);
        }
    }

}
