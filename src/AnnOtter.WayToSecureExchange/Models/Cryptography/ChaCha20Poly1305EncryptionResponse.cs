namespace AnnOtter.WayToSecureExchange.Models.Cryptography
{
    /// <summary>
    /// Represents the response after encrypting a message using the ChaCha20-Poly1305 encryption algorithm.
    /// </summary>
    public class ChaCha20Poly1305EncryptionResponse
    {
        /// <summary>
        /// Gets or sets the Nonce (a random number) used to ensure uniqueness for each encryption operation.
        /// </summary>
        public required string Nonce { get; set; }

        /// <summary>
        /// Gets or sets the Tag used for authentication and to ensure data integrity during transmission or storage.
        /// </summary>
        public required string Tag { get; set; }

        /// <summary>
        /// Gets or sets the Ciphertext, which is the encrypted text generated from the original plaintext.
        /// </summary>
        public required string Ciphertext { get; set; }
    }
}
