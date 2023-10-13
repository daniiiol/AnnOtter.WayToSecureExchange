using AnnOtter.WayToSecureExchange.Models.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace AnnOtter.WayToSecureExchange.Helpers
{
    /// <summary>
    /// Static helper class for cryptographic operations.
    /// </summary>
    public static class CryptoHelper
    {
        /// <summary>
        /// Generates a SHA-256 hash from an input string.
        /// </summary>
        /// <param name="inputString">Individual string for hashing.</param>
        /// <returns>A SHA-256 hashstring based on the inputString.</returns>
        public static string GetSha256Hash(string inputString)
        {
            var encoder = new UTF8Encoding();
            byte[] inputBytes = encoder.GetBytes(inputString);
            byte[] hashBytes = SHA256.HashData(inputBytes);

            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2")); // "x2" specifies the hexadecimal formatting
            }

            return sb.ToString();
        }

        /// <summary>
        /// Encrypts a plaintext with ChaCha20 (encryption algorithm) and Poly1305 (message authentication code).
        /// </summary>
        /// <param name="plaintext">Plaintext for encryption.</param>
        /// <param name="key">Secret key for encryption.</param>
        /// <returns>Base64 encoded encryption information such as ciphertext, nonce and generated tag.</returns>
        public static ChaCha20Poly1305EncryptionResponse EncryptChaCha20Poly1305(string plaintext, string key)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            byte[] keyBytes = Convert.FromBase64String(key); // 32-Byte Key needed!
            byte[] tagBytes = new byte[16];
            byte[] nonceBytes = new byte[12];

            RandomNumberGenerator.Fill(nonceBytes); // Filling random numbers into the nonce

            using var chacha = new ChaCha20Poly1305(keyBytes);

            byte[] ciphertext = plaintextBytes;
            chacha.Encrypt(nonceBytes, plaintextBytes, ciphertext, tagBytes, null);

            var nonceString = Convert.ToBase64String(nonceBytes);
            var ciphertextString = Convert.ToBase64String(ciphertext);
            var tagString = Convert.ToBase64String(tagBytes);

            return new ChaCha20Poly1305EncryptionResponse()
            {
                Nonce = nonceString,
                Tag = tagString,
                Ciphertext = ciphertextString
            };
        }

        /// <summary>
        /// Decrypts a ChaCha20Poly1305 encrypted ciphertext to plaintext.
        /// </summary>
        /// <param name="ciphertext">Base64 encoded ciphertext to be decrypted.</param>
        /// <param name="nonce">Base64 encoded one time nonce code.</param>
        /// <param name="key">Base64 encoded secret key.</param>
        /// <param name="tag">Base64 encoded tag, generated during encryption.</param>
        /// <returns>Decrypted a plaintext and encode it as UTF-8.</returns>
        public static string DecryptChaCha20Poly1305(string ciphertext, string nonce, string key, string tag)
        {
            byte[] ciphertextBytes = Convert.FromBase64String(ciphertext);
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] nonceBytes = Convert.FromBase64String(nonce);
            byte[] tagBytes = Convert.FromBase64String(tag);

            using var chacha = new ChaCha20Poly1305(keyBytes);
            byte[] plaintext = new byte[ciphertextBytes.Length];

            chacha.Decrypt(nonceBytes, ciphertextBytes, tagBytes, plaintext, null);

            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
