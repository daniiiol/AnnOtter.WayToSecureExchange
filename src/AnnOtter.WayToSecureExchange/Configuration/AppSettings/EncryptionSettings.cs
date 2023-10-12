namespace AnnOtter.WayToSecureExchange.Configuration.AppSettings
{
    /// <summary>
    /// Settings around encryption.
    /// </summary>
    public class EncryptionSettings
    {
        /// <summary>
        /// Important 32 byte key, used for ChaCha20 Encryption.
        /// </summary>
        public string? Key { get; set; }
    }
}
