namespace AnnOtter.WayToSecureExchange.Configuration.AppSettings
{
    /// <summary>
    /// General settings around this application.
    /// </summary>
    public class MainSettings
    {
        /// <summary>
        /// Time span until a secret exchange should be deleted.
        /// </summary>
        public TimeSpan RetentionSpan { get; set; }

        /// <summary>
        /// Maximum allowed size (in characters) for a secret. Defaults to 16384 (16 KB).
        /// </summary>
        public int MaxSecretSize { get; set; } = 16384;
    }
}
